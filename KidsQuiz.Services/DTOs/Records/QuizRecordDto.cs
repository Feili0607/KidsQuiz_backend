using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Records
{
    public class QuizRecordDto
    {
        public int Id { get; set; }
        public int KidId { get; set; }
        public int QuizId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<AnswerRecordDto> Answers { get; set; }
        public double Score { get; set; }
        public TimeSpan? TimeTaken { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public double AccuracyPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public string Feedback { get; set; }
    }

    public class AnswerRecordDto
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public bool IsCorrect { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public DateTime AnsweredAt { get; set; }
        public bool WasSkipped { get; set; }
        public int PointsEarned { get; set; }
    }
} 