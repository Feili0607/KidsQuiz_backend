using System;
using KidsQuiz.Data.Entities.ValueObjects;

namespace KidsQuiz.Services.DTOs.Quizzes
{
    public class QuizSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AgeGroup TargetAgeGroup { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public InterestCategory Category { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public int QuestionCount { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 