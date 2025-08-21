using System;

namespace KidsQuiz.Data.Models
{
    public class Redemption
    {
        public int Id { get; set; }
        public int RewardWalletId { get; set; }
        public virtual RewardWallet RewardWallet { get; set; }
        
        public int RedeemableItemId { get; set; }
        public virtual RedeemableItem RedeemableItem { get; set; }
        
        public RedemptionStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }  // Parent/Guardian Azure AD ID
        public DateTime? FulfilledAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        
        // Cost at time of redemption (in case prices change)
        public int? CoinsSpent { get; set; }
        public int? SilverGemsSpent { get; set; }
        public int? GoldCoinsSpent { get; set; }
        public int? RubiesSpent { get; set; }
        public int? SapphiresSpent { get; set; }
        public int? DiamondsSpent { get; set; }
        
        public string Notes { get; set; }
        public string RejectionReason { get; set; }
    }
    
    public enum RedemptionStatus
    {
        PendingApproval = 0,
        Approved = 1,
        Rejected = 2,
        Fulfilled = 3,
        Expired = 4,
        Cancelled = 5
    }
}