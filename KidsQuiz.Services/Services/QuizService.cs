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

namespace KidsQuiz.Services.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILLMQuizService _llmQuizService;

        public QuizService(ApplicationDbContext context, ILLMQuizService llmQuizService)
        {
            _context = context;
            _llmQuizService = llmQuizService;
        }

        public async Task<QuizDto> GetQuizAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                throw new QuizNotFoundException(id);

            return MapToDto(quiz);
        }

        public async Task<QuizDetailDto> GetQuizDetailAsync(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id);
            
            if (quiz == null)
                throw new QuizNotFoundException(id);

            return MapToDetailDto(quiz);
        }

        public async Task<IEnumerable<QuizDto>> GetAllQuizzesAsync()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return quizzes.Select(MapToDto);
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByKidIdAsync(int kidId)
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.KidId == kidId)
                .ToListAsync();
            return quizzes.Select(MapToDto);
        }

        public async Task<QuizDetailDto> GenerateQuizUsingLLMAsync(UserInfoDto userInfo, int userId)
        {
            try
            {
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

                // Map to QuizDetailDto and return
                return MapToDetailDto(quiz);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateQuizUsingLLMAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
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