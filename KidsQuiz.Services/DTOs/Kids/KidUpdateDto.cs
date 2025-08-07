using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Kids
{
    public class KidUpdateDto
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Intro { get; set; }
        public string Grade { get; set; }
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
} 