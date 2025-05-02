using System;
using System.Collections.Generic;


namespace KidsQuiz.Services.Exceptions
{
    public class QuizGenerationException : Exception
    {
        public QuizGenerationException()
            : base("Failed to generate quiz.")
        {
        }

        public QuizGenerationException(string message)
            : base(message)
        {
        }

        public QuizGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
