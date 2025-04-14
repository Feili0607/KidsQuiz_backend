using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Records
{
    public class QuizResultDto
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int KidId { get; set; }
        public string KidName { get; set; }
        public DateTime CompletedAt { get; set; }
        public double Score { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public double AccuracyPercentage { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; }
        public string Feedback { get; set; }
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public bool IsCorrect { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public int PointsEarned { get; set; }
        public string Explanation { get; set; }
    }
} 