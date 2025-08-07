using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Services.DTOs.Quizzes;

namespace KidsQuiz.Services.Interfaces
{
    public interface IQuizService
    {
        // Basic CRUD operations
        Task<QuizDto> GetQuizAsync(int id);
        Task<QuizDetailDto> GetQuizDetailAsync(int id);
        Task<IEnumerable<QuizDto>> GetAllQuizzesAsync();
        Task<IEnumerable<QuizDto>> GetQuizzesByKidIdAsync(int kidId);
        
        // OpenAI Quiz Generation
        Task<QuizDetailDto> GenerateQuizUsingLLMAsync(UserInfoDto userInfo, int userId);
    }
} 