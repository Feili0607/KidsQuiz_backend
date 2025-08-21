using System;
using System.Collections.Generic;
using System.Linq;

namespace KidsQuiz.Data.Models
{
    public class Kid
    {
        public int Id { get; set; }
        
        // Basic Information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PreferredName { get; set; }  // Nickname or preferred name
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        
        // Account Information
        public string Username { get; set; }  // Unique username for login
        public string Email { get; set; }  // Optional email for older kids
        public string AvatarUrl { get; set; }  // Profile picture or avatar
        
        // Educational Information
        public string Grade { get; set; }
        public string School { get; set; }
        public string TeacherId { get; set; }  // Primary teacher reference
        
        // Personal Details
        public string Intro { get; set; }
        public List<string> Interests { get; set; } = new List<string>();
        public List<string> FavoriteSubjects { get; set; } = new List<string>();
        public Dictionary<string, string> DynamicProperties { get; set; } = new Dictionary<string, string>();
        
        // Settings & Preferences
        public KidPreferences Preferences { get; set; }
        public string TimeZone { get; set; } = "UTC";
        public string Language { get; set; } = "en";
        
        // Account Status
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastActiveAt { get; set; }
        
        // Guardian Relationships (Max 3 guardians)
        public virtual ICollection<KidGuardianRelationship> GuardianRelationships { get; set; } = new List<KidGuardianRelationship>();
        
        // Educational Relationships
        public virtual ICollection<TeacherKidRelationship> TeacherRelationships { get; set; } = new List<TeacherKidRelationship>();
        
        // Quiz and Learning
        public virtual ICollection<QuizSolvingRecord> QuizSolvingRecords { get; set; } = new List<QuizSolvingRecord>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        
        // Reward System
        public virtual RewardWallet RewardWallet { get; set; }
        
        // User Account (for authentication)
        public virtual User User { get; set; }
        
        // Computed Properties
        public string FullName => $"{FirstName} {LastName}";
        public int Age => CalculateAge();
        public Guardian PrimaryGuardian => GuardianRelationships?.FirstOrDefault(g => g.IsPrimaryGuardian)?.Guardian;
        public int GuardianCount => GuardianRelationships?.Count(g => g.Status == RelationshipStatus.Active) ?? 0;
        
        private int CalculateAge()
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
    
    public enum Gender
    {
        Male = 0,
        Female = 1,
        Other = 2,
        PreferNotToSay = 3
    }
    
    public class KidPreferences
    {
        // Learning Preferences
        public LearningStyle PreferredLearningStyle { get; set; } = LearningStyle.Visual;
        public DifficultyPreference DifficultyPreference { get; set; } = DifficultyPreference.Adaptive;
        public int SessionDurationMinutes { get; set; } = 30;
        
        // UI Preferences
        public string Theme { get; set; } = "default";
        public int FontSize { get; set; } = 14;
        public bool EnableSoundEffects { get; set; } = true;
        public bool EnableAnimations { get; set; } = true;
        
        // Privacy Preferences
        public bool ShowProgressToClassmates { get; set; } = false;
        public bool AllowTeacherMessages { get; set; } = true;
        
        // Reward Preferences
        public bool AutoConvertCoins { get; set; } = false;
        public string PreferredRewardCategory { get; set; } = "Toys";
    }
    
    public enum LearningStyle
    {
        Visual = 0,
        Auditory = 1,
        Kinesthetic = 2,
        ReadingWriting = 3,
        Mixed = 4
    }
    
    public enum DifficultyPreference
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        Adaptive = 3  // System adjusts based on performance
    }
} 