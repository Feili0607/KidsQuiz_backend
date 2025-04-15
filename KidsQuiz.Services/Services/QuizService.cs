using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public QuizService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuizDto> GetQuizAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                throw new QuizNotFoundException(id);

            return MapToDto(quiz);
        }

        public async Task<IEnumerable<QuizDto>> GetAllQuizzesAsync()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return quizzes.Select(MapToDto);
        }

        public async Task<QuizDto> CreateQuizAsync(QuizCreateDto quizCreateDto)
        {
            var quiz = new Quiz
            {
                Title = quizCreateDto.Title,
                Description = quizCreateDto.Description,
                Content = quizCreateDto.Content,
                DifficultyLevel = quizCreateDto.DifficultyLevel,
                Labels = quizCreateDto.Labels
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return MapToDto(quiz);
        }

        public async Task UpdateQuizAsync(int id, QuizUpdateDto quizUpdateDto)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                throw new QuizNotFoundException(id);

            quiz.Title = quizUpdateDto.Title ?? quiz.Title;
            quiz.Description = quizUpdateDto.Description ?? quiz.Description;
            quiz.Content = quizUpdateDto.Content ?? quiz.Content;
            quiz.DifficultyLevel = quizUpdateDto.DifficultyLevel ?? quiz.DifficultyLevel;

            if (quizUpdateDto.Labels != null)
            {
                quiz.Labels = quizUpdateDto.Labels;
            }

            quiz.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuizAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                throw new QuizNotFoundException(id);

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task<QuizDto> GenerateQuizUsingLLMAsync(string prompt, int difficultyLevel)
        {
            // TODO: Implement LLM integration
            throw new NotImplementedException();
        }

        public async Task<QuizDto> ModifyQuizUsingLLMAsync(int quizId, string modificationPrompt)
        {
            // TODO: Implement LLM integration
            throw new NotImplementedException();
        }

        public async Task UpdateQuizRatingAsync(int quizId, double rating)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
                throw new QuizNotFoundException(quizId);

            quiz.Rating = (quiz.Rating + rating) / 2; // Simple average
            await _context.SaveChangesAsync();
        }

        public async Task AddLabelToQuizAsync(int quizId, string label)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
                throw new QuizNotFoundException(quizId);

            if (!quiz.Labels.Contains(label))
            {
                quiz.Labels.Add(label);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveLabelFromQuizAsync(int quizId, string label)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
                throw new QuizNotFoundException(quizId);

            quiz.Labels.Remove(label);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByLabelsAsync(List<string> labels)
        {
            var quizzes = await _context.Quizzes
                .Where(q => labels.All(l => q.Labels.Contains(l)))
                .ToListAsync();

            return quizzes.Select(MapToDto);
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByDifficultyAsync(int difficultyLevel)
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.DifficultyLevel == difficultyLevel)
                .ToListAsync();

            return quizzes.Select(MapToDto);
        }

        private QuizDto MapToDto(Quiz quiz)
        {
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
    }
} 