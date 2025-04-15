using System;

namespace KidsQuiz.Services.Exceptions
{
    public class QuizRecordNotFoundException : Exception
    {
        public QuizRecordNotFoundException(int recordId)
            : base($"Quiz solving record with ID {recordId} not found.")
        {
        }
    }
} 