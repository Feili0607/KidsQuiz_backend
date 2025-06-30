using System.Collections.Generic;
using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Services.DTOs.Quizzes
{
    public class QuizCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public int DifficultyLevel { get; set; }
        public List<string> Labels { get; set; }
        public List<QuestionCreateDto> Questions { get; set; }
        public AgeGroup TargetAgeGroup { get; set; }
       
        public int EstimatedDurationMinutes { get; set; }
        public bool IsGeneratedByLLM { get; set; }
        public string LLMPrompt { get; set; }
    }

    public class QuestionCreateDto
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public int Points { get; set; }
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
    }

    public class UserInfoDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Grade { get; set; }
        public string DateOfBirth { get; set; }
        public string Intro { get; set; }
    }
} 