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
            _logger.LogInformation("Retrieved {Count} quizzes for kid with ID: {KidId}", quizzes.Count, kidId);
            return quizzes.Select(MapToDto);
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