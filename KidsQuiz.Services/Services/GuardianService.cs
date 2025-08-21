using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KidsQuiz.Data;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.DTOs.Guardians;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.Exceptions;

namespace KidsQuiz.Services.Services
{
    public class GuardianService : IGuardianService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GuardianService> _logger;
        private const int MaxGuardiansPerKid = 3;

        public GuardianService(ApplicationDbContext context, ILogger<GuardianService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Guardian Management

        public async Task<GuardianDto> GetGuardianAsync(int guardianId)
        {
            var guardian = await _context.Guardians
                .Include(g => g.KidRelationships)
                    .ThenInclude(kr => kr.Kid)
                .FirstOrDefaultAsync(g => g.Id == guardianId);

            if (guardian == null)
            {
                throw new InvalidOperationException($"Guardian with ID {guardianId} not found");
            }

            return MapToGuardianDto(guardian);
        }

        public async Task<GuardianDetailDto> GetGuardianDetailAsync(int guardianId)
        {
            var guardian = await _context.Guardians
                .Include(g => g.KidRelationships)
                    .ThenInclude(kr => kr.Kid)
                .FirstOrDefaultAsync(g => g.Id == guardianId);

            if (guardian == null)
            {
                throw new InvalidOperationException($"Guardian with ID {guardianId} not found");
            }

            return MapToGuardianDetailDto(guardian);
        }

        public async Task<GuardianDto> GetGuardianByAzureIdAsync(string azureAdObjectId)
        {
            var guardian = await _context.Guardians
                .Include(g => g.KidRelationships)
                    .ThenInclude(kr => kr.Kid)
                .FirstOrDefaultAsync(g => g.AzureAdObjectId == azureAdObjectId);

            if (guardian == null)
            {
                return null;
            }

            return MapToGuardianDto(guardian);
        }

        public async Task<List<GuardianDto>> GetAllGuardiansAsync()
        {
            var guardians = await _context.Guardians
                .Include(g => g.KidRelationships)
                    .ThenInclude(kr => kr.Kid)
                .Where(g => g.IsActive)
                .ToListAsync();

            return guardians.Select(MapToGuardianDto).ToList();
        }

        public async Task<GuardianDto> CreateGuardianAsync(CreateGuardianDto dto)
        {
            // Check if guardian already exists with this Azure AD ID
            var existingGuardian = await _context.Guardians
                .FirstOrDefaultAsync(g => g.AzureAdObjectId == dto.AzureAdObjectId);

            if (existingGuardian != null)
            {
                throw new InvalidOperationException("Guardian already exists with this Azure AD ID");
            }

            // Check if email is already in use
            existingGuardian = await _context.Guardians
                .FirstOrDefaultAsync(g => g.Email == dto.Email);

            if (existingGuardian != null)
            {
                throw new InvalidOperationException("Guardian already exists with this email");
            }

            if (!Enum.TryParse<GuardianType>(dto.GuardianType, out var guardianType))
            {
                throw new ArgumentException($"Invalid guardian type: {dto.GuardianType}");
            }

            if (!Enum.TryParse<RelationshipType>(dto.RelationshipType, out var relationshipType))
            {
                throw new ArgumentException($"Invalid relationship type: {dto.RelationshipType}");
            }

            var guardian = new Guardian
            {
                AzureAdObjectId = dto.AzureAdObjectId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                AlternatePhoneNumber = dto.AlternatePhoneNumber,
                Type = guardianType,
                RelationshipType = relationshipType,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                PreferredLanguage = dto.PreferredLanguage ?? "en",
                TimeZone = dto.TimeZone ?? "UTC",
                NotificationPreferences = new NotificationPreferences(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Guardians.Add(guardian);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new guardian: {GuardianId} - {Email}", guardian.Id, guardian.Email);

            return MapToGuardianDto(guardian);
        }

        public async Task<GuardianDto> UpdateGuardianAsync(int guardianId, UpdateGuardianDto dto)
        {
            var guardian = await _context.Guardians.FindAsync(guardianId);

            if (guardian == null)
            {
                throw new InvalidOperationException($"Guardian with ID {guardianId} not found");
            }

            guardian.FirstName = dto.FirstName ?? guardian.FirstName;
            guardian.LastName = dto.LastName ?? guardian.LastName;
            guardian.PhoneNumber = dto.PhoneNumber ?? guardian.PhoneNumber;
            guardian.AlternatePhoneNumber = dto.AlternatePhoneNumber ?? guardian.AlternatePhoneNumber;
            guardian.Address = dto.Address ?? guardian.Address;
            guardian.City = dto.City ?? guardian.City;
            guardian.State = dto.State ?? guardian.State;
            guardian.PostalCode = dto.PostalCode ?? guardian.PostalCode;
            guardian.Country = dto.Country ?? guardian.Country;
            guardian.PreferredLanguage = dto.PreferredLanguage ?? guardian.PreferredLanguage;
            guardian.TimeZone = dto.TimeZone ?? guardian.TimeZone;

            if (dto.NotificationPreferences != null)
            {
                guardian.NotificationPreferences = MapToNotificationPreferences(dto.NotificationPreferences);
            }

            guardian.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated guardian: {GuardianId}", guardianId);

            return MapToGuardianDto(guardian);
        }

        public async Task<bool> DeactivateGuardianAsync(int guardianId, string reason)
        {
            var guardian = await _context.Guardians
                .Include(g => g.KidRelationships)
                .FirstOrDefaultAsync(g => g.Id == guardianId);

            if (guardian == null)
            {
                return false;
            }

            guardian.IsActive = false;
            guardian.UpdatedAt = DateTime.UtcNow;

            // Deactivate all relationships
            foreach (var relationship in guardian.KidRelationships)
            {
                relationship.Status = RelationshipStatus.Deactivated;
                relationship.DeactivatedAt = DateTime.UtcNow;
                relationship.DeactivationReason = reason;
                relationship.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Deactivated guardian: {GuardianId} for reason: {Reason}", guardianId, reason);

            return true;
        }

        public async Task<bool> ReactivateGuardianAsync(int guardianId)
        {
            var guardian = await _context.Guardians.FindAsync(guardianId);

            if (guardian == null)
            {
                return false;
            }

            guardian.IsActive = true;
            guardian.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Reactivated guardian: {GuardianId}", guardianId);

            return true;
        }

        #endregion

        #region Kid-Guardian Relationship Management

        public async Task<KidGuardianRelationshipDto> AddGuardianToKidAsync(CreateKidGuardianRelationshipDto dto)
        {
            // Check if kid exists
            var kid = await _context.Kids.FindAsync(dto.KidId);
            if (kid == null)
            {
                throw new KidNotFoundException(dto.KidId);
            }

            // Check if kid already has max guardians
            if (!await CanAddGuardianAsync(dto.KidId))
            {
                throw new InvalidOperationException($"Kid already has the maximum of {MaxGuardiansPerKid} guardians");
            }

            // Find or create guardian
            var guardian = await _context.Guardians
                .FirstOrDefaultAsync(g => g.Email == dto.GuardianEmail);

            if (guardian == null)
            {
                // Create new guardian with pending status
                if (!Enum.TryParse<GuardianType>(dto.GuardianType, out var guardianType))
                {
                    guardianType = GuardianType.Parent;
                }

                if (!Enum.TryParse<RelationshipType>(dto.RelationshipType, out var relationshipType))
                {
                    relationshipType = RelationshipType.Other;
                }

                guardian = new Guardian
                {
                    AzureAdObjectId = Guid.NewGuid().ToString(), // Temporary until they register
                    FirstName = dto.GuardianFirstName,
                    LastName = dto.GuardianLastName,
                    Email = dto.GuardianEmail,
                    PhoneNumber = dto.GuardianPhone,
                    Type = guardianType,
                    RelationshipType = relationshipType,
                    NotificationPreferences = new NotificationPreferences(),
                    IsActive = false, // Inactive until they accept invitation
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Guardians.Add(guardian);
                await _context.SaveChangesAsync();
            }

            // Check if relationship already exists
            var existingRelationship = await _context.KidGuardianRelationships
                .FirstOrDefaultAsync(r => r.KidId == dto.KidId && r.GuardianId == guardian.Id);

            if (existingRelationship != null)
            {
                throw new InvalidOperationException("This guardian is already associated with this kid");
            }

            // If this is marked as primary, ensure no other primary exists
            if (dto.IsPrimaryGuardian)
            {
                var currentPrimary = await _context.KidGuardianRelationships
                    .FirstOrDefaultAsync(r => r.KidId == dto.KidId && r.IsPrimaryGuardian);

                if (currentPrimary != null)
                {
                    currentPrimary.IsPrimaryGuardian = false;
                    currentPrimary.Priority = 2;
                    currentPrimary.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Create relationship
            var relationship = new KidGuardianRelationship
            {
                KidId = dto.KidId,
                GuardianId = guardian.Id,
                IsPrimaryGuardian = dto.IsPrimaryGuardian,
                IsEmergencyContact = dto.IsEmergencyContact,
                Priority = dto.Priority,
                Permissions = MapToGuardianPermissions(dto.Permissions),
                HasLegalCustody = dto.HasLegalCustody,
                HasEducationalRights = dto.HasEducationalRights,
                CustodyNotes = dto.CustodyNotes,
                Status = guardian.IsActive ? RelationshipStatus.Active : RelationshipStatus.Pending,
                InvitedAt = DateTime.UtcNow,
                InvitationToken = GenerateInvitationToken(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.KidGuardianRelationships.Add(relationship);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added guardian {GuardianId} to kid {KidId}", guardian.Id, dto.KidId);

            return MapToKidGuardianRelationshipDto(relationship);
        }

        public async Task<KidGuardianRelationshipDto> UpdateGuardianRelationshipAsync(int relationshipId, UpdateKidGuardianRelationshipDto dto)
        {
            var relationship = await _context.KidGuardianRelationships
                .Include(r => r.Kid)
                .Include(r => r.Guardian)
                .FirstOrDefaultAsync(r => r.Id == relationshipId);

            if (relationship == null)
            {
                throw new InvalidOperationException($"Relationship with ID {relationshipId} not found");
            }

            // If setting as primary, unset other primaries
            if (dto.IsPrimaryGuardian && !relationship.IsPrimaryGuardian)
            {
                var currentPrimary = await _context.KidGuardianRelationships
                    .FirstOrDefaultAsync(r => r.KidId == relationship.KidId && r.IsPrimaryGuardian);

                if (currentPrimary != null)
                {
                    currentPrimary.IsPrimaryGuardian = false;
                    currentPrimary.Priority = 2;
                    currentPrimary.UpdatedAt = DateTime.UtcNow;
                }
            }

            relationship.IsPrimaryGuardian = dto.IsPrimaryGuardian;
            relationship.IsEmergencyContact = dto.IsEmergencyContact;
            relationship.Priority = dto.Priority;
            relationship.Permissions = MapToGuardianPermissions(dto.Permissions);
            relationship.HasLegalCustody = dto.HasLegalCustody;
            relationship.HasEducationalRights = dto.HasEducationalRights;
            relationship.CustodyNotes = dto.CustodyNotes;
            relationship.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated relationship {RelationshipId}", relationshipId);

            return MapToKidGuardianRelationshipDto(relationship);
        }

        public async Task<bool> RemoveGuardianFromKidAsync(int kidId, int guardianId, string reason)
        {
            var relationship = await _context.KidGuardianRelationships
                .FirstOrDefaultAsync(r => r.KidId == kidId && r.GuardianId == guardianId);

            if (relationship == null)
            {
                return false;
            }

            // Check if this is the last guardian
            var guardianCount = await GetGuardianCountForKidAsync(kidId);
            if (guardianCount <= 1)
            {
                throw new InvalidOperationException("Cannot remove the last guardian for a kid");
            }

            // If removing primary guardian, promote next guardian
            if (relationship.IsPrimaryGuardian)
            {
                var nextGuardian = await _context.KidGuardianRelationships
                    .Where(r => r.KidId == kidId && r.GuardianId != guardianId && r.Status == RelationshipStatus.Active)
                    .OrderBy(r => r.Priority)
                    .FirstOrDefaultAsync();

                if (nextGuardian != null)
                {
                    nextGuardian.IsPrimaryGuardian = true;
                    nextGuardian.Priority = 1;
                    nextGuardian.UpdatedAt = DateTime.UtcNow;
                }
            }

            relationship.Status = RelationshipStatus.Deactivated;
            relationship.DeactivatedAt = DateTime.UtcNow;
            relationship.DeactivationReason = reason;
            relationship.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Removed guardian {GuardianId} from kid {KidId}", guardianId, kidId);

            return true;
        }

        public async Task<List<GuardianDto>> GetKidGuardiansAsync(int kidId)
        {
            var relationships = await _context.KidGuardianRelationships
                .Include(r => r.Guardian)
                .Where(r => r.KidId == kidId && r.Status == RelationshipStatus.Active)
                .OrderBy(r => r.Priority)
                .ToListAsync();

            return relationships.Select(r => MapToGuardianDto(r.Guardian)).ToList();
        }

        public async Task<List<KidSummaryDto>> GetGuardianKidsAsync(int guardianId)
        {
            var relationships = await _context.KidGuardianRelationships
                .Include(r => r.Kid)
                .Where(r => r.GuardianId == guardianId && r.Status == RelationshipStatus.Active)
                .ToListAsync();

            return relationships.Select(r => new KidSummaryDto
            {
                Id = r.Kid.Id,
                FirstName = r.Kid.FirstName,
                LastName = r.Kid.LastName,
                PreferredName = r.Kid.PreferredName,
                Age = r.Kid.Age,
                Grade = r.Kid.Grade,
                IsPrimaryGuardianFor = r.IsPrimaryGuardian,
                RelationshipStatus = r.Status.ToString()
            }).ToList();
        }

        public async Task<bool> SetPrimaryGuardianAsync(int kidId, int guardianId)
        {
            var relationship = await _context.KidGuardianRelationships
                .FirstOrDefaultAsync(r => r.KidId == kidId && r.GuardianId == guardianId);

            if (relationship == null)
            {
                return false;
            }

            // Unset current primary
            var currentPrimary = await _context.KidGuardianRelationships
                .FirstOrDefaultAsync(r => r.KidId == kidId && r.IsPrimaryGuardian);

            if (currentPrimary != null && currentPrimary.Id != relationship.Id)
            {
                currentPrimary.IsPrimaryGuardian = false;
                currentPrimary.Priority = 2;
                currentPrimary.UpdatedAt = DateTime.UtcNow;
            }

            relationship.IsPrimaryGuardian = true;
            relationship.Priority = 1;
            relationship.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Set guardian {GuardianId} as primary for kid {KidId}", guardianId, kidId);

            return true;
        }

        #endregion

        #region Validation and Business Rules

        public async Task<bool> CanAddGuardianAsync(int kidId)
        {
            var activeGuardianCount = await _context.KidGuardianRelationships
                .CountAsync(r => r.KidId == kidId && r.Status == RelationshipStatus.Active);

            return activeGuardianCount < MaxGuardiansPerKid;
        }

        public async Task<bool> IsGuardianOfKidAsync(int guardianId, int kidId)
        {
            return await _context.KidGuardianRelationships
                .AnyAsync(r => r.GuardianId == guardianId && 
                              r.KidId == kidId && 
                              r.Status == RelationshipStatus.Active);
        }

        public async Task<bool> IsPrimaryGuardianAsync(int guardianId, int kidId)
        {
            return await _context.KidGuardianRelationships
                .AnyAsync(r => r.GuardianId == guardianId && 
                              r.KidId == kidId && 
                              r.IsPrimaryGuardian && 
                              r.Status == RelationshipStatus.Active);
        }

        public async Task<int> GetGuardianCountForKidAsync(int kidId)
        {
            return await _context.KidGuardianRelationships
                .CountAsync(r => r.KidId == kidId && r.Status == RelationshipStatus.Active);
        }

        #endregion

        #region Helper Methods

        private GuardianDto MapToGuardianDto(Guardian guardian)
        {
            return new GuardianDto
            {
                Id = guardian.Id,
                FirstName = guardian.FirstName,
                LastName = guardian.LastName,
                Email = guardian.Email,
                PhoneNumber = guardian.PhoneNumber,
                AlternatePhoneNumber = guardian.AlternatePhoneNumber,
                GuardianType = guardian.Type.ToString(),
                RelationshipType = guardian.RelationshipType.ToString(),
                IsActive = guardian.IsActive,
                EmailVerified = guardian.EmailVerified,
                PhoneVerified = guardian.PhoneVerified,
                PreferredLanguage = guardian.PreferredLanguage,
                TimeZone = guardian.TimeZone,
                LastLoginAt = guardian.LastLoginAt,
                Kids = guardian.KidRelationships?
                    .Where(r => r.Status == RelationshipStatus.Active)
                    .Select(r => new KidSummaryDto
                    {
                        Id = r.Kid.Id,
                        FirstName = r.Kid.FirstName,
                        LastName = r.Kid.LastName,
                        PreferredName = r.Kid.PreferredName,
                        Age = r.Kid.Age,
                        Grade = r.Kid.Grade,
                        IsPrimaryGuardianFor = r.IsPrimaryGuardian,
                        RelationshipStatus = r.Status.ToString()
                    }).ToList() ?? new List<KidSummaryDto>()
            };
        }

        private GuardianDetailDto MapToGuardianDetailDto(Guardian guardian)
        {
            var dto = MapToGuardianDto(guardian);
            return new GuardianDetailDto
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                AlternatePhoneNumber = dto.AlternatePhoneNumber,
                GuardianType = dto.GuardianType,
                RelationshipType = dto.RelationshipType,
                IsActive = dto.IsActive,
                EmailVerified = dto.EmailVerified,
                PhoneVerified = dto.PhoneVerified,
                PreferredLanguage = dto.PreferredLanguage,
                TimeZone = dto.TimeZone,
                LastLoginAt = dto.LastLoginAt,
                Kids = dto.Kids,
                Address = guardian.Address,
                City = guardian.City,
                State = guardian.State,
                PostalCode = guardian.PostalCode,
                Country = guardian.Country,
                NotificationPreferences = MapToNotificationPreferencesDto(guardian.NotificationPreferences),
                CreatedAt = guardian.CreatedAt,
                UpdatedAt = guardian.UpdatedAt
            };
        }

        private KidGuardianRelationshipDto MapToKidGuardianRelationshipDto(KidGuardianRelationship relationship)
        {
            return new KidGuardianRelationshipDto
            {
                Id = relationship.Id,
                KidId = relationship.KidId,
                KidName = relationship.Kid?.FullName,
                GuardianId = relationship.GuardianId,
                GuardianName = $"{relationship.Guardian?.FirstName} {relationship.Guardian?.LastName}",
                GuardianEmail = relationship.Guardian?.Email,
                IsPrimaryGuardian = relationship.IsPrimaryGuardian,
                IsEmergencyContact = relationship.IsEmergencyContact,
                Priority = relationship.Priority,
                Permissions = MapToGuardianPermissionsDto(relationship.Permissions),
                Status = relationship.Status.ToString(),
                CreatedAt = relationship.CreatedAt
            };
        }

        private GuardianPermissions MapToGuardianPermissions(GuardianPermissionsDto dto)
        {
            if (dto == null) return new GuardianPermissions();

            return new GuardianPermissions
            {
                CanApproveRedemptions = dto.CanApproveRedemptions,
                CanAwardBonus = dto.CanAwardBonus,
                CanSetRewardGoals = dto.CanSetRewardGoals,
                CanViewRewardHistory = dto.CanViewRewardHistory,
                CanViewQuizResults = dto.CanViewQuizResults,
                CanAssignQuizzes = dto.CanAssignQuizzes,
                CanSetLearningGoals = dto.CanSetLearningGoals,
                CanViewProgressReports = dto.CanViewProgressReports,
                CanUpdateKidProfile = dto.CanUpdateKidProfile,
                CanUpdatePreferences = dto.CanUpdatePreferences,
                CanManageSchedule = dto.CanManageSchedule,
                CanMessageTeachers = dto.CanMessageTeachers,
                CanReceiveNotifications = dto.CanReceiveNotifications,
                CanAddOtherGuardians = dto.CanAddOtherGuardians,
                CanRemoveOtherGuardians = dto.CanRemoveOtherGuardians,
                CanDeactivateAccount = dto.CanDeactivateAccount,
                CanMakePurchases = dto.CanMakePurchases,
                CanViewBilling = dto.CanViewBilling,
                CanUpdatePaymentMethod = dto.CanUpdatePaymentMethod
            };
        }

        private GuardianPermissionsDto MapToGuardianPermissionsDto(GuardianPermissions permissions)
        {
            if (permissions == null) return new GuardianPermissionsDto();

            return new GuardianPermissionsDto
            {
                CanApproveRedemptions = permissions.CanApproveRedemptions,
                CanAwardBonus = permissions.CanAwardBonus,
                CanSetRewardGoals = permissions.CanSetRewardGoals,
                CanViewRewardHistory = permissions.CanViewRewardHistory,
                CanViewQuizResults = permissions.CanViewQuizResults,
                CanAssignQuizzes = permissions.CanAssignQuizzes,
                CanSetLearningGoals = permissions.CanSetLearningGoals,
                CanViewProgressReports = permissions.CanViewProgressReports,
                CanUpdateKidProfile = permissions.CanUpdateKidProfile,
                CanUpdatePreferences = permissions.CanUpdatePreferences,
                CanManageSchedule = permissions.CanManageSchedule,
                CanMessageTeachers = permissions.CanMessageTeachers,
                CanReceiveNotifications = permissions.CanReceiveNotifications,
                CanAddOtherGuardians = permissions.CanAddOtherGuardians,
                CanRemoveOtherGuardians = permissions.CanRemoveOtherGuardians,
                CanDeactivateAccount = permissions.CanDeactivateAccount,
                CanMakePurchases = permissions.CanMakePurchases,
                CanViewBilling = permissions.CanViewBilling,
                CanUpdatePaymentMethod = permissions.CanUpdatePaymentMethod
            };
        }

        private NotificationPreferences MapToNotificationPreferences(NotificationPreferencesDto dto)
        {
            if (dto == null) return new NotificationPreferences();

            return new NotificationPreferences
            {
                EmailNotifications = dto.EmailNotifications,
                SmsNotifications = dto.SmsNotifications,
                PushNotifications = dto.PushNotifications,
                DailyProgressReport = dto.DailyProgressReport,
                WeeklyProgressReport = dto.WeeklyProgressReport,
                QuizCompletionAlerts = dto.QuizCompletionAlerts,
                RedemptionRequests = dto.RedemptionRequests,
                LevelUpAlerts = dto.LevelUpAlerts,
                AchievementAlerts = dto.AchievementAlerts,
                LowBalanceAlerts = dto.LowBalanceAlerts,
                SecurityAlerts = dto.SecurityAlerts
            };
        }

        private NotificationPreferencesDto MapToNotificationPreferencesDto(NotificationPreferences preferences)
        {
            if (preferences == null) return new NotificationPreferencesDto();

            return new NotificationPreferencesDto
            {
                EmailNotifications = preferences.EmailNotifications,
                SmsNotifications = preferences.SmsNotifications,
                PushNotifications = preferences.PushNotifications,
                DailyProgressReport = preferences.DailyProgressReport,
                WeeklyProgressReport = preferences.WeeklyProgressReport,
                QuizCompletionAlerts = preferences.QuizCompletionAlerts,
                RedemptionRequests = preferences.RedemptionRequests,
                LevelUpAlerts = preferences.LevelUpAlerts,
                AchievementAlerts = preferences.AchievementAlerts,
                LowBalanceAlerts = preferences.LowBalanceAlerts,
                SecurityAlerts = preferences.SecurityAlerts
            };
        }

        private string GenerateInvitationToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        #endregion

        #region Not Implemented Methods

        public Task<string> InviteGuardianAsync(InviteGuardianDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<KidGuardianRelationshipDto> AcceptInvitationAsync(AcceptInvitationDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResendInvitationAsync(int relationshipId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CancelInvitationAsync(int relationshipId)
        {
            throw new NotImplementedException();
        }

        public Task<List<KidGuardianRelationshipDto>> GetPendingInvitationsAsync(int? kidId = null)
        {
            throw new NotImplementedException();
        }

        public Task<GuardianPermissionsDto> GetGuardianPermissionsAsync(int kidId, int guardianId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateGuardianPermissionsAsync(int kidId, int guardianId, GuardianPermissionsDto permissions)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateGuardianPermissionAsync(int guardianId, int kidId, string permission)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationPreferencesDto> GetNotificationPreferencesAsync(int guardianId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateNotificationPreferencesAsync(int guardianId, NotificationPreferencesDto preferences)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyEmailAsync(int guardianId, string verificationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyPhoneAsync(int guardianId, string verificationCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendEmailVerificationAsync(int guardianId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendPhoneVerificationAsync(int guardianId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}