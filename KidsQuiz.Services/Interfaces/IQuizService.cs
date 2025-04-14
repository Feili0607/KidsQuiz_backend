using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Services.Interfaces
{
    public interface IQuizService
    {
        Task<Quiz> GetQuizByIdAsync(int id);
        Task<List<Quiz>> GetAllQuizzesAsync();
        Task<Quiz> CreateQuizAsync(Quiz quiz);
        Task<Quiz> UpdateQuizAsync(Quiz quiz);
        Task DeleteQuizAsync(int id);
        
        // LLM Generation
        Task<Quiz> GenerateQuizUsingLLMAsync(string prompt, int difficultyLevel);
        Task<Quiz> ModifyQuizUsingLLMAsync(int quizId, string modificationPrompt);
        
        // Rating and Labeling
        Task UpdateQuizRatingAsync(int quizId, double rating);
        Task AddLabelToQuizAsync(int quizId, string label);
        Task RemoveLabelFromQuizAsync(int quizId, string label);
        
        // Filtering
        Task<List<Quiz>> GetQuizzesByLabelsAsync(List<string> labels);
        Task<List<Quiz>> GetQuizzesByDifficultyAsync(int difficultyLevel);
    }
} 