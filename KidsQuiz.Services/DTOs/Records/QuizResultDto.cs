using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Records
{
    public class QuizResultDto
    {
        public int RecordId { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double AccuracyPercentage { get; set; }
        public int TimeTakenInSeconds { get; set; }
        public Dictionary<string, object> DetailedResults { get; set; }
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