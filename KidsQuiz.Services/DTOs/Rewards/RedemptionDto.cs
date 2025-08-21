using System;

namespace KidsQuiz.Services.DTOs.Rewards
{
    public class RedemptionDto
    {
        public int Id { get; set; }
        public int RewardWalletId { get; set; }
        public int KidId { get; set; }
        public string KidName { get; set; }
        public RedeemableItemDto RedeemableItem { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? FulfilledAt { get; set; }
        
        // Cost breakdown
        public int? CoinsSpent { get; set; }
        public int? SilverGemsSpent { get; set; }
        public int? GoldCoinsSpent { get; set; }
        public int? RubiesSpent { get; set; }
        public int? SapphiresSpent { get; set; }
        public int? DiamondsSpent { get; set; }
        
        public string Notes { get; set; }
        public string RejectionReason { get; set; }
    }
    
    public class CreateRedemptionDto
    {
        public int KidId { get; set; }
        public int RedeemableItemId { get; set; }
        public string Notes { get; set; }
    }
    
    public class ApproveRedemptionDto
    {
        public int RedemptionId { get; set; }
        public bool IsApproved { get; set; }
        public string Reason { get; set; }  // Used for rejection reason
    }
    
    public class FulfillRedemptionDto
    {
        public int RedemptionId { get; set; }
        public string Notes { get; set; }
    }
    
    public class RedemptionFilterDto
    {
        public int? KidId { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}