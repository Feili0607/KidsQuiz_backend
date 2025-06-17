using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Data.ValueObjects;
using KidsQuiz.Services.DTOs.Quizzes;

namespace KidsQuiz.Services.Interfaces
{
    public interface IQuestionBankService
    {
        Task<QuestionBank> GetQuestionAsync(int id);
        Task<IEnumerable<QuestionBank>> GetQuestionsByFiltersAsync(
            AgeGroup? ageGroup = null,
            DifficultyLevel? difficultyLevel = null,
            InterestCategory? category = null,
            int? limit = null);
        Task<QuestionBank> CreateQuestionAsync(QuestionBank question);
        Task<QuestionBank> UpdateQuestionAsync(int id, QuestionBank question);
        Task DeleteQuestionAsync(int id);
        Task<Quiz> GenerateQuizAsync(
            string title,
            string description,
            AgeGroup targetAgeGroup,
            InterestCategory category,
            int difficultyLevel,
            int questionCount,
            int estimatedDurationMinutes);
        Task UpdateQuestionStatsAsync(int questionId, bool isCorrect);
    }
} 