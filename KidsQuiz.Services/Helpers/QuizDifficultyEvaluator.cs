using System.Collections.Generic;
using System.Linq;

using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Services.Helpers
{
    public static class QuizDifficultyEvaluator
    {
        public static DifficultyLevel EvaluateQuizDifficulty(
            List<DifficultyLevel> questionDifficulties,
            int totalQuestions,
            int estimatedDurationMinutes)
        {
            if (!questionDifficulties.Any())
                return DifficultyLevel.Beginner;

            var averageDifficulty = (int)questionDifficulties.Average(d => (int)d);
            var timePerQuestion = estimatedDurationMinutes / (double)totalQuestions;

            // Adjust difficulty based on time per question
            var timeAdjustment = timePerQuestion switch
            {
                < 1 => -1,
                > 3 => 1,
                _ => 0
            };

            var finalDifficulty = averageDifficulty + timeAdjustment;

            // Ensure difficulty stays within bounds
            finalDifficulty = finalDifficulty switch
            {
                < 0 => 0,
                > 3 => 3,
                _ => finalDifficulty
            };

            return (DifficultyLevel)finalDifficulty;
        }
    }
} 