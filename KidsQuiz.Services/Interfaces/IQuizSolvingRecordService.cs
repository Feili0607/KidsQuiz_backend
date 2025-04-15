using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Services.DTOs.Records;

namespace KidsQuiz.Services.Interfaces
{
    public interface IQuizSolvingRecordService
    {
        Task<IEnumerable<QuizRecordDto>> GetAllRecordsAsync();
        Task<QuizRecordDto> GetRecordAsync(int id);
        Task<IEnumerable<QuizRecordDto>> GetRecordsByKidAsync(int kidId);
        Task<IEnumerable<QuizRecordDto>> GetRecordsByQuizAsync(int quizId);
        Task<QuizRecordDto> CreateRecordAsync(QuizRecordDto recordDto);
        Task<QuizResultDto> SubmitQuizAnswersAsync(QuizRecordDto recordDto);
        Task<Dictionary<string, object>> GetKidQuizStatsAsync(int kidId);
        Task<Dictionary<string, object>> GetKidProgressAsync(int kidId);
        Task<IEnumerable<QuizSummaryDto>> GetKidRecommendationsAsync(int kidId);
    }
} 