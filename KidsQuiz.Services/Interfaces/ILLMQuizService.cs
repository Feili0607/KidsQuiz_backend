using System.Threading.Tasks;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Services.Interfaces
{
    public interface ILLMQuizService
    {
        Task<QuestionBank> GenerateQuestionAsync(string subject, string grade, string difficulty);
    }
} 