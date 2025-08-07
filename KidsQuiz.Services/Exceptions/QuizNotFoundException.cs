using System;

namespace KidsQuiz.Services.Exceptions
{
    public class QuizNotFoundException : Exception
    {
        public QuizNotFoundException(int quizId)
            : base($"Quiz with ID {quizId} not found.")
        {
        }
    }
} 