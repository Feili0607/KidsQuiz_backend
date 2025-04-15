using System;
using System.Collections.Generic;
using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Services.DTOs.Quizzes
{
    public class QuizDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public List<string> Labels { get; set; }
        public AgeGroup TargetAgeGroup { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public InterestCategory Category { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsGeneratedByLLM { get; set; }
        public string LLMPrompt { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public bool IsActive { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public int Points { get; set; }
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
    }
} 