using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KidsQuiz.Data;
using KidsQuiz.Data.Models;
using KidsQuiz.Data.ValueObjects;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.Exceptions;

namespace KidsQuiz.Services.Services
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly ApplicationDbContext _context;

        public QuestionBankService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<QuestionBank> GetQuestionAsync(int id)
        {
            var question = await _context.QuestionBanks.FindAsync(id);
            if (question == null)
                throw new NotFoundException($"Question with ID {id} not found.");
            return question;
        }

        public async Task<IEnumerable<QuestionBank>> GetQuestionsByFiltersAsync(
            AgeGroup? ageGroup = null,
            DifficultyLevel? difficultyLevel = null,
            InterestCategory? category = null,
            int? limit = null)
        {
            var query = _context.QuestionBanks.AsQueryable();

            if (ageGroup.HasValue)
                query = query.Where(q => q.TargetAgeGroup == ageGroup.Value);

            if (difficultyLevel.HasValue)
                query = query.Where(q => q.DifficultyLevel == difficultyLevel.Value);

            if (category.HasValue)
                query = query.Where(q => q.Category == category.Value);

            query = query.Where(q => q.IsActive);

            if (limit.HasValue)
                query = query.Take(limit.Value);

            return await query.ToListAsync();
        }

        public async Task<QuestionBank> CreateQuestionAsync(QuestionBank question)
        {
            question.CreatedAt = DateTime.UtcNow;
            question.IsActive = true;
            question.UsageCount = 0;
            question.SuccessRate = 0;

            _context.QuestionBanks.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<QuestionBank> UpdateQuestionAsync(int id, QuestionBank question)
        {
            var existingQuestion = await _context.QuestionBanks.FindAsync(id);
            if (existingQuestion == null)
                throw new NotFoundException($"Question with ID {id} not found.");

            existingQuestion.Text = question.Text;
            existingQuestion.Options = question.Options;
            existingQuestion.CorrectAnswerIndex = question.CorrectAnswerIndex;
            existingQuestion.Explanation = question.Explanation;
            existingQuestion.DifficultyLevel = question.DifficultyLevel;
            existingQuestion.Points = question.Points;
            existingQuestion.ImageUrl = question.ImageUrl;
            existingQuestion.AudioUrl = question.AudioUrl;
            existingQuestion.TargetAgeGroup = question.TargetAgeGroup;
            existingQuestion.Category = question.Category;
            existingQuestion.Tags = question.Tags;
            existingQuestion.ModifiedAt = DateTime.UtcNow;
            existingQuestion.IsActive = question.IsActive;

            await _context.SaveChangesAsync();
            return existingQuestion;
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _context.QuestionBanks.FindAsync(id);
            if (question == null)
                throw new NotFoundException($"Question with ID {id} not found.");

            _context.QuestionBanks.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task<Quiz> GenerateQuizAsync(
            string title,
            string description,
            AgeGroup targetAgeGroup,
            InterestCategory category,
            int difficultyLevel,
            int questionCount,
            int estimatedDurationMinutes)
        {
            // Get questions from the question bank
            var questions = await GetQuestionsByFiltersAsync(
                ageGroup: targetAgeGroup,
                category: category,
                difficultyLevel: (DifficultyLevel)difficultyLevel,
                limit: questionCount * 2); // Get more questions than needed to ensure variety

            if (!questions.Any())
                throw new NotFoundException("No suitable questions found for the specified criteria.");

            // Randomly select questions
            var random = new Random();
            var selectedQuestions = questions
                .OrderBy(q => random.Next())
                .Take(questionCount)
                .ToList();

            // Create the quiz
            var quiz = new Quiz
            {
                Title = title,
                Description = description,
                Content = $"Quiz generated from question bank for {targetAgeGroup} {category}",
                DifficultyLevel = difficultyLevel,
                Rating = 0,
                RatingCount = 0,
                CreatedAt = DateTime.UtcNow,
                Labels = new List<string> { 
                    targetAgeGroup.ToString().ToLower(),
                    category.ToString().ToLower(),
                    $"difficulty-{difficultyLevel}"
                },
                EstimatedDurationMinutes = estimatedDurationMinutes,
                IsGeneratedByLLM = false,
                Questions = selectedQuestions.Select(q => new Question
                {
                    Text = q.Text,
                    Options = q.Options,
                    CorrectAnswerIndex = q.CorrectAnswerIndex,
                    Explanation = q.Explanation,
                    DifficultyLevel = q.DifficultyLevel,
                    Points = q.Points,
                    ImageUrl = q.ImageUrl,
                    AudioUrl = q.AudioUrl,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            // Update usage count for selected questions
            foreach (var question in selectedQuestions)
            {
                question.UsageCount++;
                _context.QuestionBanks.Update(question);
            }
            await _context.SaveChangesAsync();

            return quiz;
        }

        public async Task UpdateQuestionStatsAsync(int questionId, bool isCorrect)
        {
            var question = await _context.QuestionBanks.FindAsync(questionId);
            if (question == null)
                throw new NotFoundException($"Question with ID {questionId} not found.");

            // Update success rate
            var totalAttempts = question.UsageCount;
            var currentSuccessCount = (int)(question.SuccessRate * totalAttempts);
            var newSuccessCount = currentSuccessCount + (isCorrect ? 1 : 0);
            question.SuccessRate = (double)newSuccessCount / (totalAttempts + 1);

            await _context.SaveChangesAsync();
        }
    }
} 