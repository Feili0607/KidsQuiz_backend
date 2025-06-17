using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Services.Helpers
{
    public static class GradeToAgeGroupMapper
    {
        public static AgeGroup MapGradeToAgeGroup(string grade)
        {
            // Remove any non-numeric characters and convert to lowercase
            var normalizedGrade = grade.ToLower().Replace("th", "").Replace("st", "").Replace("nd", "").Replace("rd", "");
            
            if (int.TryParse(normalizedGrade, out int gradeNumber))
            {
                return gradeNumber switch
                {
                    <= 0 => AgeGroup.Preschool,      // Pre-K, Kindergarten
                    <= 3 => AgeGroup.EarlyElementary, // Grades 1-3
                    <= 5 => AgeGroup.LateElementary,  // Grades 4-5
                    <= 8 => AgeGroup.MiddleSchool,    // Grades 6-8
                    _ => AgeGroup.HighSchool          // Grades 9-12
                };
            }

            // Handle special cases
            return grade.ToLower() switch
            {
                "pre-k" or "preschool" or "kindergarten" => AgeGroup.Preschool,
                "k" or "kinder" => AgeGroup.Preschool,
                _ => AgeGroup.EarlyElementary // Default to early elementary if grade format is unknown
            };
        }

        public static DifficultyLevel GetRecommendedDifficultyLevel(string grade)
        {
            var ageGroup = MapGradeToAgeGroup(grade);
            
            return ageGroup switch
            {
                AgeGroup.Preschool => DifficultyLevel.Beginner,
                AgeGroup.EarlyElementary => DifficultyLevel.Beginner,
                AgeGroup.LateElementary => DifficultyLevel.Intermediate,
                AgeGroup.MiddleSchool => DifficultyLevel.Advanced,
                AgeGroup.HighSchool => DifficultyLevel.Expert,
                _ => DifficultyLevel.Beginner
            };
        }
    }
} 