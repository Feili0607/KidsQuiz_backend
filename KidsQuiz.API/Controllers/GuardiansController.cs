using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Guardians;
using KidsQuiz.Data.Models;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/guardians")]
    [Authorize]
    public class GuardiansController : ControllerBase
    {
        private readonly IGuardianService _guardianService;
        private readonly ILogger<GuardiansController> _logger;

        public GuardiansController(IGuardianService guardianService, ILogger<GuardiansController> logger)
        {
            _guardianService = guardianService;
            _logger = logger;
        }

        #region Guardian Management

        /// <summary>
        /// Get guardian by ID
        /// </summary>
        [HttpGet("{guardianId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<GuardianDto>> GetGuardian(int guardianId)
        {
            try
            {
                // Verify the guardian is accessing their own data or is an admin
                if (!await ValidateGuardianAccess(guardianId))
                {
                    return Forbid("You can only access your own guardian profile");
                }

                var guardian = await _guardianService.GetGuardianAsync(guardianId);
                return Ok(guardian);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guardian {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while retrieving the guardian");
            }
        }

        /// <summary>
        /// Get detailed guardian information
        /// </summary>
        [HttpGet("{guardianId}/detail")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<GuardianDetailDto>> GetGuardianDetail(int guardianId)
        {
            try
            {
                if (!await ValidateGuardianAccess(guardianId))
                {
                    return Forbid();
                }

                var guardian = await _guardianService.GetGuardianDetailAsync(guardianId);
                return Ok(guardian);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guardian detail {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while retrieving guardian details");
            }
        }

        /// <summary>
        /// Get guardian by Azure AD ID
        /// </summary>
        [HttpGet("azure/{azureAdObjectId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<GuardianDto>> GetGuardianByAzureId(string azureAdObjectId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId != azureAdObjectId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var guardian = await _guardianService.GetGuardianByAzureIdAsync(azureAdObjectId);
                if (guardian == null)
                {
                    return NotFound("Guardian not found");
                }

                return Ok(guardian);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guardian by Azure ID");
                return StatusCode(500, "An error occurred while retrieving the guardian");
            }
        }

        /// <summary>
        /// Get all guardians (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<GuardianDto>>> GetAllGuardians()
        {
            try
            {
                var guardians = await _guardianService.GetAllGuardiansAsync();
                return Ok(guardians);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all guardians");
                return StatusCode(500, "An error occurred while retrieving guardians");
            }
        }

        /// <summary>
        /// Create a new guardian
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<GuardianDto>> CreateGuardian([FromBody] CreateGuardianDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var guardian = await _guardianService.CreateGuardianAsync(dto);
                return CreatedAtAction(nameof(GetGuardian), new { guardianId = guardian.Id }, guardian);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating guardian");
                return StatusCode(500, "An error occurred while creating the guardian");
            }
        }

        /// <summary>
        /// Update guardian information
        /// </summary>
        [HttpPut("{guardianId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<GuardianDto>> UpdateGuardian(int guardianId, [FromBody] UpdateGuardianDto dto)
        {
            try
            {
                if (!await ValidateGuardianAccess(guardianId))
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var guardian = await _guardianService.UpdateGuardianAsync(guardianId, dto);
                return Ok(guardian);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating guardian {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while updating the guardian");
            }
        }

        /// <summary>
        /// Deactivate a guardian
        /// </summary>
        [HttpPost("{guardianId}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateGuardian(int guardianId, [FromQuery] string reason)
        {
            try
            {
                var result = await _guardianService.DeactivateGuardianAsync(guardianId, reason);
                if (!result)
                {
                    return NotFound("Guardian not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating guardian {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while deactivating the guardian");
            }
        }

        /// <summary>
        /// Reactivate a guardian
        /// </summary>
        [HttpPost("{guardianId}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ReactivateGuardian(int guardianId)
        {
            try
            {
                var result = await _guardianService.ReactivateGuardianAsync(guardianId);
                if (!result)
                {
                    return NotFound("Guardian not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reactivating guardian {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while reactivating the guardian");
            }
        }

        #endregion

        #region Kid-Guardian Relationships

        /// <summary>
        /// Add a guardian to a kid (max 3 guardians per kid)
        /// </summary>
        [HttpPost("relationships")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<KidGuardianRelationshipDto>> AddGuardianToKid([FromBody] CreateKidGuardianRelationshipDto dto)
        {
            try
            {
                // Check if the current user has permission to add guardians for this kid
                if (!User.IsInRole("Admin"))
                {
                    var hasPermission = await ValidateGuardianPermissionForKid(dto.KidId, "CanAddOtherGuardians");
                    if (!hasPermission)
                    {
                        return Forbid("You don't have permission to add guardians for this kid");
                    }
                }

                var relationship = await _guardianService.AddGuardianToKidAsync(dto);
                return Ok(relationship);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding guardian to kid {KidId}", dto.KidId);
                return StatusCode(500, "An error occurred while adding the guardian");
            }
        }

        /// <summary>
        /// Update guardian relationship permissions
        /// </summary>
        [HttpPut("relationships/{relationshipId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<KidGuardianRelationshipDto>> UpdateGuardianRelationship(
            int relationshipId, 
            [FromBody] UpdateKidGuardianRelationshipDto dto)
        {
            try
            {
                var relationship = await _guardianService.UpdateGuardianRelationshipAsync(relationshipId, dto);
                return Ok(relationship);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating relationship {RelationshipId}", relationshipId);
                return StatusCode(500, "An error occurred while updating the relationship");
            }
        }

        /// <summary>
        /// Remove a guardian from a kid
        /// </summary>
        [HttpDelete("relationships/kid/{kidId}/guardian/{guardianId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult> RemoveGuardianFromKid(int kidId, int guardianId, [FromQuery] string reason)
        {
            try
            {
                if (!User.IsInRole("Admin"))
                {
                    var hasPermission = await ValidateGuardianPermissionForKid(kidId, "CanRemoveOtherGuardians");
                    if (!hasPermission)
                    {
                        return Forbid("You don't have permission to remove guardians for this kid");
                    }
                }

                var result = await _guardianService.RemoveGuardianFromKidAsync(kidId, guardianId, reason);
                if (!result)
                {
                    return NotFound("Relationship not found");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing guardian {GuardianId} from kid {KidId}", guardianId, kidId);
                return StatusCode(500, "An error occurred while removing the guardian");
            }
        }

        /// <summary>
        /// Get all guardians for a kid
        /// </summary>
        [HttpGet("kid/{kidId}")]
        [Authorize(Roles = "Kid,Parent,Guardian,Teacher,Admin")]
        public async Task<ActionResult<List<GuardianDto>>> GetKidGuardians(int kidId)
        {
            try
            {
                // Verify access to this kid's information
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var guardians = await _guardianService.GetKidGuardiansAsync(kidId);
                return Ok(guardians);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guardians for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while retrieving guardians");
            }
        }

        /// <summary>
        /// Get all kids for a guardian
        /// </summary>
        [HttpGet("{guardianId}/kids")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<List<KidSummaryDto>>> GetGuardianKids(int guardianId)
        {
            try
            {
                if (!await ValidateGuardianAccess(guardianId))
                {
                    return Forbid();
                }

                var kids = await _guardianService.GetGuardianKidsAsync(guardianId);
                return Ok(kids);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting kids for guardian {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while retrieving kids");
            }
        }

        /// <summary>
        /// Set primary guardian for a kid
        /// </summary>
        [HttpPost("kid/{kidId}/primary/{guardianId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult> SetPrimaryGuardian(int kidId, int guardianId)
        {
            try
            {
                if (!User.IsInRole("Admin"))
                {
                    // Only current primary guardian or admin can change primary guardian
                    var isPrimary = await _guardianService.IsPrimaryGuardianAsync(
                        await GetCurrentGuardianId(), kidId);
                    
                    if (!isPrimary)
                    {
                        return Forbid("Only the primary guardian can change the primary guardian");
                    }
                }

                var result = await _guardianService.SetPrimaryGuardianAsync(kidId, guardianId);
                if (!result)
                {
                    return NotFound("Relationship not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting primary guardian {GuardianId} for kid {KidId}", guardianId, kidId);
                return StatusCode(500, "An error occurred while setting the primary guardian");
            }
        }

        #endregion

        #region Guardian Invitations

        /// <summary>
        /// Invite a guardian for a kid
        /// </summary>
        [HttpPost("invite")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<string>> InviteGuardian([FromBody] InviteGuardianDto dto)
        {
            try
            {
                if (!User.IsInRole("Admin"))
                {
                    var hasPermission = await ValidateGuardianPermissionForKid(dto.KidId, "CanAddOtherGuardians");
                    if (!hasPermission)
                    {
                        return Forbid("You don't have permission to invite guardians for this kid");
                    }
                }

                // Check if kid can have more guardians
                if (!await _guardianService.CanAddGuardianAsync(dto.KidId))
                {
                    return BadRequest("This kid already has the maximum number of guardians (3)");
                }

                var invitationToken = await _guardianService.InviteGuardianAsync(dto);
                return Ok(new { invitationToken, message = "Invitation sent successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inviting guardian for kid {KidId}", dto.KidId);
                return StatusCode(500, "An error occurred while sending the invitation");
            }
        }

        /// <summary>
        /// Accept guardian invitation
        /// </summary>
        [HttpPost("accept-invitation")]
        [AllowAnonymous]
        public async Task<ActionResult<KidGuardianRelationshipDto>> AcceptInvitation([FromBody] AcceptInvitationDto dto)
        {
            try
            {
                var relationship = await _guardianService.AcceptInvitationAsync(dto);
                return Ok(relationship);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting invitation");
                return StatusCode(500, "An error occurred while accepting the invitation");
            }
        }

        /// <summary>
        /// Get pending invitations
        /// </summary>
        [HttpGet("invitations/pending")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<List<KidGuardianRelationshipDto>>> GetPendingInvitations([FromQuery] int? kidId = null)
        {
            try
            {
                if (kidId.HasValue && !await ValidateKidAccess(kidId.Value))
                {
                    return Forbid();
                }

                var invitations = await _guardianService.GetPendingInvitationsAsync(kidId);
                return Ok(invitations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending invitations");
                return StatusCode(500, "An error occurred while retrieving invitations");
            }
        }

        #endregion

        #region Permissions

        /// <summary>
        /// Get guardian permissions for a kid
        /// </summary>
        [HttpGet("permissions/kid/{kidId}/guardian/{guardianId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<GuardianPermissionsDto>> GetGuardianPermissions(int kidId, int guardianId)
        {
            try
            {
                if (!await ValidateGuardianAccess(guardianId) && !await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var permissions = await _guardianService.GetGuardianPermissionsAsync(kidId, guardianId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for guardian {GuardianId} and kid {KidId}", guardianId, kidId);
                return StatusCode(500, "An error occurred while retrieving permissions");
            }
        }

        /// <summary>
        /// Update guardian permissions
        /// </summary>
        [HttpPut("permissions/kid/{kidId}/guardian/{guardianId}")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult> UpdateGuardianPermissions(
            int kidId, 
            int guardianId, 
            [FromBody] GuardianPermissionsDto permissions)
        {
            try
            {
                if (!User.IsInRole("Admin"))
                {
                    // Only primary guardian can update permissions
                    var isPrimary = await _guardianService.IsPrimaryGuardianAsync(
                        await GetCurrentGuardianId(), kidId);
                    
                    if (!isPrimary)
                    {
                        return Forbid("Only the primary guardian can update permissions");
                    }
                }

                var result = await _guardianService.UpdateGuardianPermissionsAsync(kidId, guardianId, permissions);
                if (!result)
                {
                    return NotFound("Relationship not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permissions for guardian {GuardianId} and kid {KidId}", guardianId, kidId);
                return StatusCode(500, "An error occurred while updating permissions");
            }
        }

        #endregion

        #region Notification Preferences

        /// <summary>
        /// Get notification preferences
        /// </summary>
        [HttpGet("{guardianId}/notifications")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<NotificationPreferencesDto>> GetNotificationPreferences(int guardianId)
        {
            try
            {
                if (!await ValidateGuardianAccess(guardianId))
                {
                    return Forbid();
                }

                var preferences = await _guardianService.GetNotificationPreferencesAsync(guardianId);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification preferences for guardian {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while retrieving notification preferences");
            }
        }

        /// <summary>
        /// Update notification preferences
        /// </summary>
        [HttpPut("{guardianId}/notifications")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult> UpdateNotificationPreferences(
            int guardianId, 
            [FromBody] NotificationPreferencesDto preferences)
        {
            try
            {
                if (!await ValidateGuardianAccess(guardianId))
                {
                    return Forbid();
                }

                var result = await _guardianService.UpdateNotificationPreferencesAsync(guardianId, preferences);
                if (!result)
                {
                    return NotFound("Guardian not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preferences for guardian {GuardianId}", guardianId);
                return StatusCode(500, "An error occurred while updating notification preferences");
            }
        }

        #endregion

        #region Helper Methods

        private async Task<bool> ValidateGuardianAccess(int guardianId)
        {
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            var azureId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(azureId))
            {
                return false;
            }

            var guardian = await _guardianService.GetGuardianByAzureIdAsync(azureId);
            return guardian != null && guardian.Id == guardianId;
        }

        private async Task<bool> ValidateKidAccess(int kidId)
        {
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            if (User.IsInRole("Kid"))
            {
                // TODO: Implement kid access validation
                return true;
            }

            if (User.IsInRole("Parent") || User.IsInRole("Guardian"))
            {
                var guardianId = await GetCurrentGuardianId();
                if (guardianId > 0)
                {
                    return await _guardianService.IsGuardianOfKidAsync(guardianId, kidId);
                }
            }

            if (User.IsInRole("Teacher"))
            {
                // TODO: Implement teacher access validation
                return true;
            }

            return false;
        }

        private async Task<bool> ValidateGuardianPermissionForKid(int kidId, string permission)
        {
            var guardianId = await GetCurrentGuardianId();
            if (guardianId <= 0)
            {
                return false;
            }

            return await _guardianService.ValidateGuardianPermissionAsync(guardianId, kidId, permission);
        }

        private async Task<int> GetCurrentGuardianId()
        {
            var azureId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(azureId))
            {
                return 0;
            }

            var guardian = await _guardianService.GetGuardianByAzureIdAsync(azureId);
            return guardian?.Id ?? 0;
        }

        #endregion
    }
}