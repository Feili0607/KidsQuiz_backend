using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Services.DTOs.Guardians;

namespace KidsQuiz.Services.Interfaces
{
    public interface IGuardianService
    {
        // Guardian Management
        Task<GuardianDto> GetGuardianAsync(int guardianId);
        Task<GuardianDetailDto> GetGuardianDetailAsync(int guardianId);
        Task<GuardianDto> GetGuardianByAzureIdAsync(string azureAdObjectId);
        Task<List<GuardianDto>> GetAllGuardiansAsync();
        Task<GuardianDto> CreateGuardianAsync(CreateGuardianDto dto);
        Task<GuardianDto> UpdateGuardianAsync(int guardianId, UpdateGuardianDto dto);
        Task<bool> DeactivateGuardianAsync(int guardianId, string reason);
        Task<bool> ReactivateGuardianAsync(int guardianId);
        
        // Kid-Guardian Relationship Management
        Task<KidGuardianRelationshipDto> AddGuardianToKidAsync(CreateKidGuardianRelationshipDto dto);
        Task<KidGuardianRelationshipDto> UpdateGuardianRelationshipAsync(int relationshipId, UpdateKidGuardianRelationshipDto dto);
        Task<bool> RemoveGuardianFromKidAsync(int kidId, int guardianId, string reason);
        Task<List<GuardianDto>> GetKidGuardiansAsync(int kidId);
        Task<List<KidSummaryDto>> GetGuardianKidsAsync(int guardianId);
        Task<bool> SetPrimaryGuardianAsync(int kidId, int guardianId);
        
        // Guardian Invitation System
        Task<string> InviteGuardianAsync(InviteGuardianDto dto);
        Task<KidGuardianRelationshipDto> AcceptInvitationAsync(AcceptInvitationDto dto);
        Task<bool> ResendInvitationAsync(int relationshipId);
        Task<bool> CancelInvitationAsync(int relationshipId);
        Task<List<KidGuardianRelationshipDto>> GetPendingInvitationsAsync(int? kidId = null);
        
        // Permissions Management
        Task<GuardianPermissionsDto> GetGuardianPermissionsAsync(int kidId, int guardianId);
        Task<bool> UpdateGuardianPermissionsAsync(int kidId, int guardianId, GuardianPermissionsDto permissions);
        Task<bool> ValidateGuardianPermissionAsync(int guardianId, int kidId, string permission);
        
        // Validation and Business Rules
        Task<bool> CanAddGuardianAsync(int kidId);  // Check if kid has less than 3 guardians
        Task<bool> IsGuardianOfKidAsync(int guardianId, int kidId);
        Task<bool> IsPrimaryGuardianAsync(int guardianId, int kidId);
        Task<int> GetGuardianCountForKidAsync(int kidId);
        
        // Notification Management
        Task<NotificationPreferencesDto> GetNotificationPreferencesAsync(int guardianId);
        Task<bool> UpdateNotificationPreferencesAsync(int guardianId, NotificationPreferencesDto preferences);
        
        // Verification
        Task<bool> VerifyEmailAsync(int guardianId, string verificationToken);
        Task<bool> VerifyPhoneAsync(int guardianId, string verificationCode);
        Task<bool> SendEmailVerificationAsync(int guardianId);
        Task<bool> SendPhoneVerificationAsync(int guardianId);
    }
}