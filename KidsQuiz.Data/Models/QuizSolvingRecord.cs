using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class QuizSolvingRecord
    {
        public int Id { get; set; }
        public int KidId { get; set; }
        public int QuizId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int Score { get; set; }
        public string Answers { get; set; } // JSON string containing answers
        public int TimeTakenInSeconds { get; set; }
        
        // Navigation properties
        public Kid Kid { get; set; }
        public Quiz Quiz { get; set; }
    }

    public class AnswerRecord
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public bool IsCorrect { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }
} 