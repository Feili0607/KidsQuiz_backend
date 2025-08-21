using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Rewards;
using KidsQuiz.Data.Models;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/rewards")]
    [Authorize]
    public class RewardsController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        private readonly ILogger<RewardsController> _logger;

        public RewardsController(IRewardService rewardService, ILogger<RewardsController> logger)
        {
            _rewardService = rewardService;
            _logger = logger;
        }

        #region Wallet Management

        /// <summary>
        /// Get reward wallet for a specific kid
        /// </summary>
        [HttpGet("wallet/{kidId}")]
        [Authorize(Roles = "Kid,Parent,Guardian,Teacher,Admin")]
        public async Task<ActionResult<RewardWalletDto>> GetWallet(int kidId)
        {
            try
            {
                // Validate access based on role
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid("You don't have access to this kid's wallet");
                }

                var wallet = await _rewardService.GetWalletAsync(kidId);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while retrieving the wallet");
            }
        }

        /// <summary>
        /// Get wallet summary for a kid
        /// </summary>
        [HttpGet("wallet/{kidId}/summary")]
        [Authorize(Roles = "Kid,Parent,Guardian,Teacher,Admin")]
        public async Task<ActionResult<RewardWalletSummaryDto>> GetWalletSummary(int kidId)
        {
            try
            {
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var summary = await _rewardService.GetWalletSummaryAsync(kidId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet summary for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while retrieving the wallet summary");
            }
        }

        /// <summary>
        /// Get all wallets (Admin only)
        /// </summary>
        [HttpGet("wallets")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<RewardWalletDto>>> GetAllWallets()
        {
            try
            {
                var wallets = await _rewardService.GetAllWalletsAsync();
                return Ok(wallets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all wallets");
                return StatusCode(500, "An error occurred while retrieving wallets");
            }
        }

        #endregion

        #region Earning Rewards

        /// <summary>
        /// Award rewards to a kid
        /// </summary>
        [HttpPost("earn")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<RewardWalletDto>> EarnReward([FromBody] EarnRewardDto earnDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var wallet = await _rewardService.EarnRewardAsync(earnDto);
                _logger.LogInformation("Reward earned for kid {KidId}: {Amount} {Type}", 
                    earnDto.KidId, earnDto.Amount, earnDto.RewardType);
                
                return Ok(wallet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error earning reward for kid {KidId}", earnDto.KidId);
                return StatusCode(500, "An error occurred while processing the reward");
            }
        }

        /// <summary>
        /// Process quiz completion reward
        /// </summary>
        [HttpPost("quiz-completion")]
        [Authorize(Roles = "Kid,Teacher,Admin")]
        public async Task<ActionResult<RewardWalletDto>> ProcessQuizReward(
            [FromQuery] int kidId, 
            [FromQuery] int quizId, 
            [FromQuery] double scorePercentage)
        {
            try
            {
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var wallet = await _rewardService.ProcessQuizCompletionRewardAsync(kidId, quizId, scorePercentage);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing quiz reward for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while processing the quiz reward");
            }
        }

        /// <summary>
        /// Process daily login reward
        /// </summary>
        [HttpPost("daily-login/{kidId}")]
        [Authorize(Roles = "Kid,Admin")]
        public async Task<ActionResult<RewardWalletDto>> ProcessDailyLogin(int kidId)
        {
            try
            {
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var wallet = await _rewardService.ProcessDailyLoginRewardAsync(kidId);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing daily login for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while processing the daily login reward");
            }
        }

        /// <summary>
        /// Process streak reward
        /// </summary>
        [HttpPost("streak/{kidId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RewardWalletDto>> ProcessStreakReward(int kidId, [FromQuery] int streakDays)
        {
            try
            {
                var wallet = await _rewardService.ProcessStreakRewardAsync(kidId, streakDays);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing streak reward for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while processing the streak reward");
            }
        }

        #endregion

        #region Currency Conversion

        /// <summary>
        /// Convert between currencies
        /// </summary>
        [HttpPost("convert")]
        [Authorize(Roles = "Kid,Parent,Guardian,Admin")]
        public async Task<ActionResult<RewardWalletDto>> ConvertCurrency([FromBody] ConvertCurrencyDto convertDto)
        {
            try
            {
                if (!await ValidateKidAccess(convertDto.KidId))
                {
                    return Forbid();
                }

                var wallet = await _rewardService.ConvertCurrencyAsync(convertDto);
                return Ok(wallet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting currency for kid {KidId}", convertDto.KidId);
                return StatusCode(500, "An error occurred while converting currency");
            }
        }

        /// <summary>
        /// Get conversion rates
        /// </summary>
        [HttpGet("conversion-rates")]
        [AllowAnonymous]
        public ActionResult<Dictionary<string, decimal>> GetConversionRates()
        {
            var rates = _rewardService.GetConversionRates();
            return Ok(rates);
        }

        #endregion

        #region Transaction History

        /// <summary>
        /// Get transaction history
        /// </summary>
        [HttpGet("transactions")]
        [Authorize(Roles = "Kid,Parent,Guardian,Teacher,Admin")]
        public async Task<ActionResult<TransactionHistoryDto>> GetTransactionHistory([FromQuery] TransactionFilterDto filter)
        {
            try
            {
                if (filter.KidId.HasValue && !await ValidateKidAccess(filter.KidId.Value))
                {
                    return Forbid();
                }

                var history = await _rewardService.GetTransactionHistoryAsync(filter);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction history");
                return StatusCode(500, "An error occurred while retrieving transaction history");
            }
        }

        /// <summary>
        /// Get recent transactions for a kid
        /// </summary>
        [HttpGet("transactions/recent/{kidId}")]
        [Authorize(Roles = "Kid,Parent,Guardian,Teacher,Admin")]
        public async Task<ActionResult<List<RewardTransactionDto>>> GetRecentTransactions(int kidId, [FromQuery] int count = 10)
        {
            try
            {
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var transactions = await _rewardService.GetRecentTransactionsAsync(kidId, count);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent transactions for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while retrieving recent transactions");
            }
        }

        #endregion

        #region Redeemable Items

        /// <summary>
        /// Get available redeemable items for a kid
        /// </summary>
        [HttpGet("items/available/{kidId}")]
        [Authorize(Roles = "Kid,Parent,Guardian,Admin")]
        public async Task<ActionResult<List<RedeemableItemDto>>> GetAvailableItems(int kidId)
        {
            try
            {
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var items = await _rewardService.GetAvailableItemsAsync(kidId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available items for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while retrieving available items");
            }
        }

        /// <summary>
        /// Get redeemable item by ID
        /// </summary>
        [HttpGet("items/{id}")]
        [Authorize]
        public async Task<ActionResult<RedeemableItemDto>> GetRedeemableItem(int id)
        {
            try
            {
                var item = await _rewardService.GetRedeemableItemAsync(id);
                return Ok(item);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting redeemable item {ItemId}", id);
                return StatusCode(500, "An error occurred while retrieving the item");
            }
        }

        /// <summary>
        /// Create new redeemable item (Admin only)
        /// </summary>
        [HttpPost("items")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RedeemableItemDto>> CreateRedeemableItem([FromBody] CreateRedeemableItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = await _rewardService.CreateRedeemableItemAsync(dto);
                return CreatedAtAction(nameof(GetRedeemableItem), new { id = item.Id }, item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating redeemable item");
                return StatusCode(500, "An error occurred while creating the item");
            }
        }

        /// <summary>
        /// Update redeemable item (Admin only)
        /// </summary>
        [HttpPut("items/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RedeemableItemDto>> UpdateRedeemableItem(int id, [FromBody] UpdateRedeemableItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = await _rewardService.UpdateRedeemableItemAsync(id, dto);
                return Ok(item);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating redeemable item {ItemId}", id);
                return StatusCode(500, "An error occurred while updating the item");
            }
        }

        /// <summary>
        /// Delete redeemable item (Admin only)
        /// </summary>
        [HttpDelete("items/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRedeemableItem(int id)
        {
            try
            {
                var result = await _rewardService.DeleteRedeemableItemAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting redeemable item {ItemId}", id);
                return StatusCode(500, "An error occurred while deleting the item");
            }
        }

        /// <summary>
        /// Get items by category
        /// </summary>
        [HttpGet("items/category/{category}")]
        [Authorize]
        public async Task<ActionResult<List<RedeemableItemDto>>> GetItemsByCategory(string category, [FromQuery] int? kidId = null)
        {
            try
            {
                if (kidId.HasValue && !await ValidateKidAccess(kidId.Value))
                {
                    return Forbid();
                }

                var items = await _rewardService.GetItemsByCategoryAsync(category, kidId);
                return Ok(items);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting items by category {Category}", category);
                return StatusCode(500, "An error occurred while retrieving items");
            }
        }

        #endregion

        #region Redemptions

        /// <summary>
        /// Request a redemption
        /// </summary>
        [HttpPost("redemptions/request")]
        [Authorize(Roles = "Kid,Parent,Guardian,Admin")]
        public async Task<ActionResult<RedemptionDto>> RequestRedemption([FromBody] CreateRedemptionDto dto)
        {
            try
            {
                if (!await ValidateKidAccess(dto.KidId))
                {
                    return Forbid();
                }

                var redemption = await _rewardService.RequestRedemptionAsync(dto);
                return Ok(redemption);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting redemption for kid {KidId}", dto.KidId);
                return StatusCode(500, "An error occurred while requesting redemption");
            }
        }

        /// <summary>
        /// Approve or reject a redemption
        /// </summary>
        [HttpPost("redemptions/approve")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<RedemptionDto>> ApproveRedemption([FromBody] ApproveRedemptionDto dto)
        {
            try
            {
                var approverUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var redemption = await _rewardService.ApproveRedemptionAsync(dto, approverUserId);
                return Ok(redemption);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving redemption {RedemptionId}", dto.RedemptionId);
                return StatusCode(500, "An error occurred while approving redemption");
            }
        }

        /// <summary>
        /// Fulfill a redemption
        /// </summary>
        [HttpPost("redemptions/fulfill")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<RedemptionDto>> FulfillRedemption([FromBody] FulfillRedemptionDto dto)
        {
            try
            {
                var redemption = await _rewardService.FulfillRedemptionAsync(dto);
                return Ok(redemption);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fulfilling redemption {RedemptionId}", dto.RedemptionId);
                return StatusCode(500, "An error occurred while fulfilling redemption");
            }
        }

        /// <summary>
        /// Cancel a redemption
        /// </summary>
        [HttpPost("redemptions/{redemptionId}/cancel")]
        [Authorize(Roles = "Kid,Parent,Guardian,Admin")]
        public async Task<ActionResult<RedemptionDto>> CancelRedemption(int redemptionId, [FromQuery] string reason)
        {
            try
            {
                var redemption = await _rewardService.CancelRedemptionAsync(redemptionId, reason);
                return Ok(redemption);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling redemption {RedemptionId}", redemptionId);
                return StatusCode(500, "An error occurred while canceling redemption");
            }
        }

        /// <summary>
        /// Get pending redemptions
        /// </summary>
        [HttpGet("redemptions/pending")]
        [Authorize(Roles = "Parent,Guardian,Admin")]
        public async Task<ActionResult<List<RedemptionDto>>> GetPendingRedemptions([FromQuery] int? kidId = null)
        {
            try
            {
                if (kidId.HasValue && !await ValidateKidAccess(kidId.Value))
                {
                    return Forbid();
                }

                var redemptions = await _rewardService.GetPendingRedemptionsAsync(kidId);
                return Ok(redemptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending redemptions");
                return StatusCode(500, "An error occurred while retrieving pending redemptions");
            }
        }

        /// <summary>
        /// Get redemption history
        /// </summary>
        [HttpGet("redemptions/history")]
        [Authorize]
        public async Task<ActionResult<List<RedemptionDto>>> GetRedemptionHistory([FromQuery] RedemptionFilterDto filter)
        {
            try
            {
                if (filter.KidId.HasValue && !await ValidateKidAccess(filter.KidId.Value))
                {
                    return Forbid();
                }

                var redemptions = await _rewardService.GetRedemptionHistoryAsync(filter);
                return Ok(redemptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting redemption history");
                return StatusCode(500, "An error occurred while retrieving redemption history");
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get reward statistics for a kid
        /// </summary>
        [HttpGet("statistics/{kidId}")]
        [Authorize(Roles = "Kid,Parent,Guardian,Teacher,Admin")]
        public async Task<ActionResult<Dictionary<string, object>>> GetRewardStatistics(int kidId)
        {
            try
            {
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var stats = await _rewardService.GetRewardStatisticsAsync(kidId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reward statistics for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }

        /// <summary>
        /// Get monthly earnings for a kid
        /// </summary>
        [HttpGet("earnings/monthly/{kidId}")]
        [Authorize(Roles = "Kid,Parent,Guardian,Teacher,Admin")]
        public async Task<ActionResult<Dictionary<string, int>>> GetMonthlyEarnings(
            int kidId, 
            [FromQuery] int year, 
            [FromQuery] int month)
        {
            try
            {
                if (!await ValidateKidAccess(kidId))
                {
                    return Forbid();
                }

                var earnings = await _rewardService.GetMonthlyEarningsAsync(kidId, year, month);
                return Ok(earnings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly earnings for kid {KidId}", kidId);
                return StatusCode(500, "An error occurred while retrieving monthly earnings");
            }
        }

        #endregion

        #region Helper Methods

        private async Task<bool> ValidateKidAccess(int kidId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(userId))
            {
                return false;
            }

            // Parse the role
            if (!Enum.TryParse<UserRole>(userRole, out var role))
            {
                return false;
            }

            switch (role)
            {
                case UserRole.Admin:
                    return true;
                    
                case UserRole.Kid:
                    // Kids can only access their own wallet
                    // TODO: Implement logic to check if userId matches kidId
                    return true;
                    
                case UserRole.Parent:
                case UserRole.Guardian:
                    // Parents/Guardians can access their children's wallets
                    // TODO: Implement logic to check parent-kid relationship
                    return true;
                    
                case UserRole.Teacher:
                    // Teachers can access their students' wallets
                    // TODO: Implement logic to check teacher-student relationship
                    return true;
                    
                default:
                    return false;
            }
        }

        #endregion
    }
}