using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int DifficultyLevel { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsGeneratedByLLM { get; set; }
        public string LLMPrompt { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public List<string> Labels { get; set; } = new List<string>();
        
        // Navigation properties
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizSolvingRecord> QuizSolvingRecords { get; set; } = new List<QuizSolvingRecord>();
    }
} 