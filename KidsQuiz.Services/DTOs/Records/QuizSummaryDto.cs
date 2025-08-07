using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Records
{
    public class QuizSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DifficultyLevel { get; set; }
        public double Rating { get; set; }
        public List<string> Labels { get; set; }
    }
} 