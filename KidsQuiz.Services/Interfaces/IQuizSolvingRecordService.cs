using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Services.Interfaces
{
    public interface IQuizSolvingRecordService
    {
        Task<QuizSolvingRecord> GetRecordByIdAsync(int id);
        Task<List<QuizSolvingRecord>> GetRecordsByKidIdAsync(int kidId);
        Task<List<QuizSolvingRecord>> GetRecordsByQuizIdAsync(int quizId);
        Task<QuizSolvingRecord> StartQuizSolvingAsync(int kidId, int quizId);
        Task<QuizSolvingRecord> CompleteQuizSolvingAsync(int recordId, string answers, int score, int timeTakenInSeconds);
        
        // Analytics
        Task<double> GetAverageScoreByKidAsync(int kidId);
        Task<double> GetAverageScoreByQuizAsync(int quizId);
        Task<List<QuizSolvingRecord>> GetRecentRecordsByKidAsync(int kidId, int count);
    }
} 