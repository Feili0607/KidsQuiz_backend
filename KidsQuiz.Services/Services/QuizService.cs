using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Quizzes;
using KidsQuiz.Services.Exceptions;
using KidsQuiz.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KidsQuiz.Services.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILLMQuizService _llmQuizService;
        private readonly ILogger<QuizService> _logger;

        public QuizService(ApplicationDbContext context, ILLMQuizService llmQuizService, ILogger<QuizService> logger)
        {
            _context = context;
            _llmQuizService = llmQuizService;
            _logger = logger;
        }

        public async Task<QuizDto> GetQuizAsync(int id)
        {
            _logger.LogInformation("Getting quiz with ID: {QuizId}", id);
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found", id);
                throw new QuizNotFoundException(id);
            }

            _logger.LogInformation("Successfully retrieved quiz with ID: {QuizId}", id);
            return MapToDto(quiz);
        }

        public async Task<QuizDetailDto> GetQuizDetailAsync(int id)
        {
            _logger.LogInformation("Getting quiz detail with ID: {QuizId}", id);
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id);
            
            if (quiz == null)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found for detail", id);
                throw new QuizNotFoundException(id);
            }

            _logger.LogInformation("Successfully retrieved quiz detail with ID: {QuizId}", id);
            return MapToDetailDto(quiz);
        }

        public async Task<IEnumerable<QuizDto>> GetAllQuizzesAsync()
        {
            _logger.LogInformation("Getting all quizzes");
            var quizzes = await _context.Quizzes.ToListAsync();
            _logger.LogInformation("Retrieved {Count} quizzes", quizzes.Count);
            return quizzes.Select(MapToDto);
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByKidIdAsync(int kidId)
        {
            _logger.LogInformation("Getting quizzes for kid with ID: {KidId}", kidId);
            var quizzes = await _context.Quizzes
                .Where(q => q.KidId == kidId)
                .ToListAsync();
            _logger.LogInformation("Retrieved {Count} quizzes for kid with ID: {KidId}", quizzes.Count(), kidId);
            return quizzes.Select(MapToDto);
        }

        public async Task<QuizDto> UpdateQuizAsync(int id, QuizUpdateDto quizUpdateDto)
        {
            _logger.LogInformation("Updating quiz with ID: {QuizId}", id);
            _logger.LogInformation("Update data received: Title='{Title}', Description='{Description}', Content='{Content}', DifficultyLevel={DifficultyLevel}, Labels={Labels}", 
                quizUpdateDto.Title, quizUpdateDto.Description, quizUpdateDto.Content, quizUpdateDto.DifficultyLevel, 
                quizUpdateDto.Labels != null ? string.Join(",", quizUpdateDto.Labels) : "null");
            
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found for update", id);
                throw new QuizNotFoundException(id);
            }

            _logger.LogInformation("Current quiz data: Title='{Title}', DifficultyLevel={DifficultyLevel}", quiz.Title, quiz.DifficultyLevel);

            // Update only the fields that are provided
            if (!string.IsNullOrEmpty(quizUpdateDto.Title))
            {
                quiz.Title = quizUpdateDto.Title;
                _logger.LogInformation("Updated Title from '{OldTitle}' to '{NewTitle}'", quiz.Title, quizUpdateDto.Title);
            }
            
            if (!string.IsNullOrEmpty(quizUpdateDto.Description))
            {
                quiz.Description = quizUpdateDto.Description;
                _logger.LogInformation("Updated Description");
            }
            
            if (!string.IsNullOrEmpty(quizUpdateDto.Content))
            {
                quiz.Content = quizUpdateDto.Content;
                _logger.LogInformation("Updated Content");
            }
            
            if (quizUpdateDto.DifficultyLevel.HasValue)
            {
                var oldDifficulty = quiz.DifficultyLevel;
                quiz.DifficultyLevel = quizUpdateDto.DifficultyLevel.Value;
                _logger.LogInformation("Updated DifficultyLevel from {OldDifficulty} to {NewDifficulty}", oldDifficulty, quiz.DifficultyLevel);
            }
            else
            {
                _logger.LogInformation("DifficultyLevel not provided in update request");
            }
            
            if (quizUpdateDto.Labels != null && quizUpdateDto.Labels.Any())
            {
                quiz.Labels = quizUpdateDto.Labels;
                _logger.LogInformation("Updated Labels to: {Labels}", string.Join(",", quiz.Labels));
            }

            // Set modification timestamp
            quiz.ModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("About to save changes. Final quiz data: Title='{Title}', DifficultyLevel={DifficultyLevel}", quiz.Title, quiz.DifficultyLevel);
            
            await _context.SaveChangesAsync();
            
            // Refresh the entity to get the latest data from the database
            await _context.Entry(quiz).ReloadAsync();
            
            _logger.LogInformation("Successfully updated quiz with ID: {QuizId}. Final DifficultyLevel: {DifficultyLevel}", id, quiz.DifficultyLevel);
            return MapToDto(quiz);
        }

        public async Task<bool> FixQuizAnswerIndicesAsync(int quizId)
        {
            _logger.LogInformation("Fixing answer indices for quiz with ID: {QuizId}", quizId);
            
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);
                
            if (quiz == null)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found for answer index fix", quizId);
                throw new QuizNotFoundException(quizId);
            }

            var hasChanges = false;
            
            foreach (var question in quiz.Questions)
            {
                _logger.LogInformation("Checking question {QuestionId}: {QuestionText}", question.Id, question.Text);
                _logger.LogInformation("Options: [{Options}]", string.Join(", ", question.Options));
                _logger.LogInformation("Current CorrectAnswerIndex: {CurrentIndex}", question.CorrectAnswerIndex);
                
                // Check if the index is valid
                if (question.CorrectAnswerIndex < 0 || question.CorrectAnswerIndex >= question.Options.Count)
                {
                    _logger.LogWarning("Invalid CorrectAnswerIndex {Index} for question {QuestionId}. Options count: {OptionsCount}. Fixing...", 
                        question.CorrectAnswerIndex, question.Id, question.Options.Count);
                    
                    // Try to find the correct answer by looking at the explanation
                    var correctAnswer = ExtractCorrectAnswerFromExplanation(question.Explanation, question.Options);
                    if (correctAnswer >= 0)
                    {
                        question.CorrectAnswerIndex = correctAnswer;
                        _logger.LogInformation("Fixed CorrectAnswerIndex to {FixedIndex} based on explanation", correctAnswer);
                        hasChanges = true;
                    }
                    else
                    {
                        // Default to first option if we can't determine
                        question.CorrectAnswerIndex = 0;
                        _logger.LogWarning("Could not determine correct answer from explanation. Setting to 0.");
                        hasChanges = true;
                    }
                }
            }
            
            if (hasChanges)
            {
                quiz.ModifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully fixed answer indices for quiz {QuizId}", quizId);
            }
            else
            {
                _logger.LogInformation("No answer index fixes needed for quiz {QuizId}", quizId);
            }
            
            return hasChanges;
        }

        private int ExtractCorrectAnswerFromExplanation(string explanation, List<string> options)
        {
            if (string.IsNullOrEmpty(explanation) || options == null || !options.Any())
                return -1;
                
            // Try to find the correct answer in the explanation
            foreach (var option in options)
            {
                if (explanation.Contains(option, StringComparison.OrdinalIgnoreCase))
                {
                    return options.IndexOf(option);
                }
            }
            
            return -1;
        }

        public async Task<QuizDetailDto> GenerateQuizUsingLLMAsync(UserInfoDto userInfo, int userId)
        {
            try
            {
                _logger.LogInformation("Generating quiz using LLM for user {UserId}", userId);
                _logger.LogInformation("User info: Name={Name}, Grade={Grade}, Subject={Subject}", 
                    userInfo?.Name, userInfo?.Grade, userInfo?.Subject);
                
                // Call the LLM service to generate the full quiz content
                var quiz = await _llmQuizService.GenerateFullQuizAsync(userInfo);

                // Associate the quiz with the user
                quiz.KidId = userId;

                // Ensure all required fields are set
                if (quiz.Questions != null)
                {
                    foreach (var question in quiz.Questions)
                    {
                        question.AudioUrl = question.AudioUrl ?? "";
                        question.ImageUrl = question.ImageUrl ?? "";
                        question.Explanation = question.Explanation ?? "";
                        question.Text = question.Text ?? "";
                        if (question.Options == null)
                            question.Options = new List<string>();
                    }
                }

                // Save the newly generated quiz to the database
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully generated and saved quiz with ID: {QuizId} for user {UserId}", quiz.Id, userId);

                // Map to QuizDetailDto and return
                return MapToDetailDto(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateQuizUsingLLMAsync for user {UserId}: {Message}", userId, ex.Message);
                throw;
            }
        }

        private QuizDto MapToDto(Quiz quiz)
        {
            if (quiz == null) return null;
            return new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                Content = quiz.Content,
                DifficultyLevel = quiz.DifficultyLevel,
                Rating = quiz.Rating,
                CreatedAt = quiz.CreatedAt,
                ModifiedAt = quiz.ModifiedAt,
                Labels = quiz.Labels
            };
        }

        private QuizDetailDto MapToDetailDto(Quiz quiz)
        {
            if (quiz == null) return null;
            return new QuizDetailDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = quiz.Questions?.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Options = q.Options,
                    CorrectAnswerIndex = q.CorrectAnswerIndex,
                    Explanation = q.Explanation,
                    DifficultyLevel = q.DifficultyLevel,
                    Points = q.Points,
                    ImageUrl = q.ImageUrl,
                    AudioUrl = q.AudioUrl
                }).ToList(),
                Labels = quiz.Labels,
                TargetAgeGroup = 0, // Set if available
                DifficultyLevel = (KidsQuiz.Data.ValueObjects.DifficultyLevel)quiz.DifficultyLevel,
                Category = 0, // Set if available
                Rating = quiz.Rating,
                RatingCount = quiz.RatingCount,
                CreatedAt = quiz.CreatedAt,
                ModifiedAt = quiz.ModifiedAt,
                IsGeneratedByLLM = quiz.IsGeneratedByLLM,
                LLMPrompt = quiz.LLMPrompt,
                EstimatedDurationMinutes = quiz.EstimatedDurationMinutes,
                IsActive = true // or set as needed
            };
        }
    }
} 