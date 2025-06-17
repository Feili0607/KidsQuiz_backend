using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Services.Interfaces
{
    public interface IExternalQuizService
    {
        Task<IEnumerable<QuestionBank>> FetchQuestionsFromOpenTDBAsync(
            int count = 10,
            string category = null,
            string difficulty = null);
    }
} 