using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Services.DTOs.Quizzes;

namespace KidsQuiz.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizDto> GetQuizAsync(int id);
        Task<IEnumerable<QuizDto>> GetAllQuizzesAsync();
        Task<QuizDto> CreateQuizAsync(QuizCreateDto quizCreateDto);
        Task UpdateQuizAsync(int id, QuizUpdateDto quizUpdateDto);
        Task DeleteQuizAsync(int id);
        
        // LLM Generation
        Task<QuizDto> GenerateQuizUsingLLMAsync(string prompt, int difficultyLevel);
        Task<QuizDto> ModifyQuizUsingLLMAsync(int quizId, string modificationPrompt);
        
        // Rating and Labeling
        Task UpdateQuizRatingAsync(int quizId, double rating);
        Task AddLabelToQuizAsync(int quizId, string label);
        Task RemoveLabelFromQuizAsync(int quizId, string label);
        
        // Filtering
        Task<IEnumerable<QuizDto>> GetQuizzesByLabelsAsync(List<string> labels);
        Task<IEnumerable<QuizDto>> GetQuizzesByDifficultyAsync(int difficultyLevel);
    }
} 