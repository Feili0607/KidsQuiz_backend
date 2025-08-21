using System;

namespace KidsQuiz.Data.Models
{
    public class KidGuardianRelationship
    {
        public int Id { get; set; }
        
        // Foreign Keys
        public int KidId { get; set; }
        public virtual Kid Kid { get; set; }
        
        public int GuardianId { get; set; }
        public virtual Guardian Guardian { get; set; }
        
        // Relationship Details
        public bool IsPrimaryGuardian { get; set; } = false;  // Only one primary guardian per kid
        public bool IsEmergencyContact { get; set; } = false;
        public int Priority { get; set; } = 1;  // 1 = Primary, 2 = Secondary, 3 = Tertiary
        
        // Permissions - What can this guardian do?
        public GuardianPermissions Permissions { get; set; }
        
        // Legal/Custody Information
        public bool HasLegalCustody { get; set; } = true;
        public bool HasEducationalRights { get; set; } = true;
        public string CustodyNotes { get; set; }  // Any special custody arrangements
        
        // Status
        public RelationshipStatus Status { get; set; } = RelationshipStatus.Active;
        public DateTime? InvitedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public string InvitationToken { get; set; }  // For email invitation tracking
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public string DeactivationReason { get; set; }
    }
    
    public class GuardianPermissions
    {
        // Reward System Permissions
        public bool CanApproveRedemptions { get; set; } = true;
        public bool CanAwardBonus { get; set; } = false;
        public bool CanSetRewardGoals { get; set; } = true;
        public bool CanViewRewardHistory { get; set; } = true;
        
        // Quiz and Learning Permissions
        public bool CanViewQuizResults { get; set; } = true;
        public bool CanAssignQuizzes { get; set; } = true;
        public bool CanSetLearningGoals { get; set; } = true;
        public bool CanViewProgressReports { get; set; } = true;
        
        // Profile Management Permissions
        public bool CanUpdateKidProfile { get; set; } = false;
        public bool CanUpdatePreferences { get; set; } = true;
        public bool CanManageSchedule { get; set; } = true;
        
        // Communication Permissions
        public bool CanMessageTeachers { get; set; } = true;
        public bool CanReceiveNotifications { get; set; } = true;
        
        // Account Management Permissions
        public bool CanAddOtherGuardians { get; set; } = false;  // Usually only primary guardian
        public bool CanRemoveOtherGuardians { get; set; } = false;
        public bool CanDeactivateAccount { get; set; } = false;
        
        // Financial Permissions (if applicable)
        public bool CanMakePurchases { get; set; } = false;
        public bool CanViewBilling { get; set; } = false;
        public bool CanUpdatePaymentMethod { get; set; } = false;
    }
    
    public enum RelationshipStatus
    {
        Pending = 0,      // Invitation sent but not accepted
        Active = 1,       // Active relationship
        Inactive = 2,     // Temporarily inactive
        Deactivated = 3,  // Permanently deactivated
        Rejected = 4      // Invitation rejected
    }
}