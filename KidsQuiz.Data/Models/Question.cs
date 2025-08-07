using System;
using System.Collections.Generic;
using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Data.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public int Points { get; set; }
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Navigation property
        public Quiz Quiz { get; set; }
    }
} 