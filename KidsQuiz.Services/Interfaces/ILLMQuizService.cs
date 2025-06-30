using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.DTOs.Quizzes;

namespace KidsQuiz.Services.Interfaces
{
    public interface ILLMQuizService
    {
        Task<QuestionBank> GenerateQuestionAsync(string subject, string grade, string difficulty);
        Task<Quiz> GenerateFullQuizAsync(UserInfoDto userInfo);
    }
} 