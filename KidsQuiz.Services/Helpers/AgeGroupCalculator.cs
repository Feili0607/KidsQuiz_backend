using System;
using KidsQuiz.Data.Entities.ValueObjects;

namespace KidsQuiz.Services.Helpers
{
    public static class AgeGroupCalculator
    {
        public static AgeGroup CalculateAgeGroup(DateTime dateOfBirth)
        {
            var age = CalculateAge(dateOfBirth);

            return age switch
            {
                <= 5 => AgeGroup.Preschool,
                <= 8 => AgeGroup.EarlyElementary,
                <= 11 => AgeGroup.LateElementary,
                <= 14 => AgeGroup.MiddleSchool,
                _ => AgeGroup.HighSchool
            };
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
} 