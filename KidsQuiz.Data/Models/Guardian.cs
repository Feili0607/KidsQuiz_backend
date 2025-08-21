using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class Guardian
    {
        public int Id { get; set; }
        
        // Azure AD Integration
        public string AzureAdObjectId { get; set; }  // Azure Entra ID Object ID
        
        // Personal Information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        
        // Guardian Type
        public GuardianType Type { get; set; }  // Parent, Guardian, Grandparent, etc.
        public RelationshipType RelationshipType { get; set; }  // Mother, Father, Aunt, Uncle, etc.
        
        // Address Information (optional)
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        
        // Account Status
        public bool IsActive { get; set; } = true;
        public bool EmailVerified { get; set; } = false;
        public bool PhoneVerified { get; set; } = false;
        
        // Preferences
        public NotificationPreferences NotificationPreferences { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public string TimeZone { get; set; } = "UTC";
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation Properties
        public virtual ICollection<KidGuardianRelationship> KidRelationships { get; set; } = new List<KidGuardianRelationship>();
    }
    
    public enum GuardianType
    {
        Parent = 0,
        LegalGuardian = 1,
        Grandparent = 2,
        RelativeCaregiver = 3,  // Aunt, Uncle, etc.
        FosterParent = 4,
        Other = 5
    }
    
    public enum RelationshipType
    {
        Mother = 0,
        Father = 1,
        Stepmother = 2,
        Stepfather = 3,
        Grandmother = 4,
        Grandfather = 5,
        Aunt = 6,
        Uncle = 7,
        LegalGuardian = 8,
        FosterMother = 9,
        FosterFather = 10,
        Other = 11
    }
    
    public class NotificationPreferences
    {
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool PushNotifications { get; set; } = true;
        
        // Specific notification types
        public bool DailyProgressReport { get; set; } = true;
        public bool WeeklyProgressReport { get; set; } = true;
        public bool QuizCompletionAlerts { get; set; } = true;
        public bool RedemptionRequests { get; set; } = true;
        public bool LevelUpAlerts { get; set; } = true;
        public bool AchievementAlerts { get; set; } = true;
        public bool LowBalanceAlerts { get; set; } = false;
        public bool SecurityAlerts { get; set; } = true;
    }
}