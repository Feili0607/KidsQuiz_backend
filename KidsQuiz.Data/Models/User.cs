using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string AzureAdObjectId { get; set; }  // Azure Entra ID Object ID
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        
        // For Kids role
        public int? KidId { get; set; }
        public virtual Kid Kid { get; set; }
        
        // For Parent/Guardian role - can have multiple kids
        public virtual ICollection<ParentKidRelationship> ParentKidRelationships { get; set; } = new List<ParentKidRelationship>();
        
        // For Teacher role - can have multiple classes/kids
        public virtual ICollection<TeacherKidRelationship> TeacherKidRelationships { get; set; } = new List<TeacherKidRelationship>();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
    
    public enum UserRole
    {
        Kid = 0,
        Parent = 1,
        Guardian = 2,
        Teacher = 3,
        Admin = 4
    }
    
    public class ParentKidRelationship
    {
        public int Id { get; set; }
        public int ParentUserId { get; set; }
        public virtual User ParentUser { get; set; }
        public int KidId { get; set; }
        public virtual Kid Kid { get; set; }
        public bool IsPrimary { get; set; } = false;
        public bool CanApproveRedemptions { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
    
    public class TeacherKidRelationship
    {
        public int Id { get; set; }
        public int TeacherUserId { get; set; }
        public virtual User TeacherUser { get; set; }
        public int KidId { get; set; }
        public virtual Kid Kid { get; set; }
        public string ClassName { get; set; }
        public string SchoolYear { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}