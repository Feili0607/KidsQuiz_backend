using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Quizzes
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int DifficultyLevel { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public List<string> Labels { get; set; }
    }
} 