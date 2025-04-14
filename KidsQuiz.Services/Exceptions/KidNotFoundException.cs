using System;

namespace KidsQuiz.Services.Exceptions
{
    public class KidNotFoundException : Exception
    {
        public KidNotFoundException(int kidId)
            : base($"Kid with ID {kidId} was not found.")
        {
            KidId = kidId;
        }

        public int KidId { get; }
    }
} 