using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; } // JSON string containing questions and answers
        public int DifficultyLevel { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsGeneratedByLLM { get; set; }
        public string LLMPrompt { get; set; }
        
        // Labels for categorization
        public List<string> Labels { get; set; } = new List<string>();
        
        // Navigation property for quiz solving records
        public ICollection<QuizSolvingRecord> QuizSolvingRecords { get; set; } = new List<QuizSolvingRecord>();
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
    }
} 