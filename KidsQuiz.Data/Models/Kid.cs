using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class Kid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Intro { get; set; }
        public string Grade { get; set; }
        
        // Dictionary to store dynamic properties (hobbies, interests, etc.)
        public Dictionary<string, string> DynamicProperties { get; set; } = new Dictionary<string, string>();
        
        // Navigation property for quiz solving records
        public ICollection<QuizSolvingRecord> QuizSolvingRecords { get; set; } = new List<QuizSolvingRecord>();
        
        // Navigation property for quizzes
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
} 