using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.Exceptions
{
    public class QuizRecordValidationException : Exception
    {
        public Dictionary<string, string> ValidationErrors { get; }

        public QuizRecordValidationException(Dictionary<string, string> validationErrors)
            : base("Quiz record validation failed.")
        {
            ValidationErrors = validationErrors;
        }
    }
} 