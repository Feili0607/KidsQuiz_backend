using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Quizzes
{
    public class QuizUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public int? DifficultyLevel { get; set; }
        public List<string>? Labels { get; set; }
    }
} 