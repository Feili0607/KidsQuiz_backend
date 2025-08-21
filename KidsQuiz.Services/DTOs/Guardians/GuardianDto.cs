using System;
using System.Collections.Generic;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Services.DTOs.Guardians
{
    public class GuardianDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string GuardianType { get; set; }
        public string RelationshipType { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneVerified { get; set; }
        public string PreferredLanguage { get; set; }
        public string TimeZone { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<KidSummaryDto> Kids { get; set; } = new List<KidSummaryDto>();
    }
    
    public class GuardianDetailDto : GuardianDto
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public NotificationPreferencesDto NotificationPreferences { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class CreateGuardianDto
    {
        public string AzureAdObjectId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string GuardianType { get; set; }
        public string RelationshipType { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public string TimeZone { get; set; } = "UTC";
    }
    
    public class UpdateGuardianDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PreferredLanguage { get; set; }
        public string TimeZone { get; set; }
        public NotificationPreferencesDto NotificationPreferences { get; set; }
    }
    
    public class NotificationPreferencesDto
    {
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool PushNotifications { get; set; } = true;
        public bool DailyProgressReport { get; set; } = true;
        public bool WeeklyProgressReport { get; set; } = true;
        public bool QuizCompletionAlerts { get; set; } = true;
        public bool RedemptionRequests { get; set; } = true;
        public bool LevelUpAlerts { get; set; } = true;
        public bool AchievementAlerts { get; set; } = true;
        public bool LowBalanceAlerts { get; set; } = false;
        public bool SecurityAlerts { get; set; } = true;
    }
    
    public class KidSummaryDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }
        public int Age { get; set; }
        public string Grade { get; set; }
        public bool IsPrimaryGuardianFor { get; set; }
        public string RelationshipStatus { get; set; }
    }
}