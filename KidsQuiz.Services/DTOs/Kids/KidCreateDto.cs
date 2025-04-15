using System;
using System.Collections.Generic;
using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Services.DTOs.Kids
{
    public class KidCreateDto
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Intro { get; set; }
        public List<InterestCategory> Interests { get; set; }
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
} 