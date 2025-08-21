using System;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Services.DTOs.Guardians
{
    public class KidGuardianRelationshipDto
    {
        public int Id { get; set; }
        public int KidId { get; set; }
        public string KidName { get; set; }
        public int GuardianId { get; set; }
        public string GuardianName { get; set; }
        public string GuardianEmail { get; set; }
        public bool IsPrimaryGuardian { get; set; }
        public bool IsEmergencyContact { get; set; }
        public int Priority { get; set; }
        public GuardianPermissionsDto Permissions { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class CreateKidGuardianRelationshipDto
    {
        public int KidId { get; set; }
        public string GuardianEmail { get; set; }  // Used to invite guardian
        public string GuardianFirstName { get; set; }
        public string GuardianLastName { get; set; }
        public string GuardianPhone { get; set; }
        public string GuardianType { get; set; }
        public string RelationshipType { get; set; }
        public bool IsPrimaryGuardian { get; set; }
        public bool IsEmergencyContact { get; set; }
        public int Priority { get; set; } = 2;  // Default to secondary
        public GuardianPermissionsDto Permissions { get; set; }
        public bool HasLegalCustody { get; set; } = true;
        public bool HasEducationalRights { get; set; } = true;
        public string CustodyNotes { get; set; }
    }
    
    public class UpdateKidGuardianRelationshipDto
    {
        public bool IsPrimaryGuardian { get; set; }
        public bool IsEmergencyContact { get; set; }
        public int Priority { get; set; }
        public GuardianPermissionsDto Permissions { get; set; }
        public bool HasLegalCustody { get; set; }
        public bool HasEducationalRights { get; set; }
        public string CustodyNotes { get; set; }
    }
    
    public class GuardianPermissionsDto
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
        public bool CanAddOtherGuardians { get; set; } = false;
        public bool CanRemoveOtherGuardians { get; set; } = false;
        public bool CanDeactivateAccount { get; set; } = false;
        
        // Financial Permissions
        public bool CanMakePurchases { get; set; } = false;
        public bool CanViewBilling { get; set; } = false;
        public bool CanUpdatePaymentMethod { get; set; } = false;
    }
    
    public class InviteGuardianDto
    {
        public int KidId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RelationshipType { get; set; }
        public bool IsPrimaryGuardian { get; set; }
        public string InvitationMessage { get; set; }
    }
    
    public class AcceptInvitationDto
    {
        public string InvitationToken { get; set; }
        public string AzureAdObjectId { get; set; }
        public bool AcceptTerms { get; set; }
    }
}