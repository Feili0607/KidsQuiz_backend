using System;
using System.Collections.Generic;
using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Data.Models
{
    public class QuestionBank
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public int Points { get; set; }
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
        public AgeGroup TargetAgeGroup { get; set; }
        public InterestCategory Category { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public double SuccessRate { get; set; }
    }
} 