using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.Exceptions
{
    public class QuizValidationException : Exception
    {
        public QuizValidationException(string message, IEnumerable<string> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
        }

        public IEnumerable<string> ValidationErrors { get; }
    }
} 