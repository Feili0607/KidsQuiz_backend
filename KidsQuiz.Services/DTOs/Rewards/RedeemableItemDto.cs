using System;

namespace KidsQuiz.Services.DTOs.Rewards
{
    public class RedeemableItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        
        // Cost in different currencies
        public int? CoinsCost { get; set; }
        public int? SilverGemsCost { get; set; }
        public int? GoldCoinsCost { get; set; }
        public int? RubiesCost { get; set; }
        public int? SapphiresCost { get; set; }
        public int? DiamondsCost { get; set; }
        
        public int MinimumLevel { get; set; }
        public int QuantityAvailable { get; set; }
        public bool IsActive { get; set; }
        public bool RequiresParentApproval { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public DateTime? ExpiresAt { get; set; }
        
        // Calculated fields
        public bool IsAffordable { get; set; }
        public bool IsLevelUnlocked { get; set; }
    }
    
    public class CreateRedeemableItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int? CoinsCost { get; set; }
        public int? SilverGemsCost { get; set; }
        public int? GoldCoinsCost { get; set; }
        public int? RubiesCost { get; set; }
        public int? SapphiresCost { get; set; }
        public int? DiamondsCost { get; set; }
        public int MinimumLevel { get; set; } = 1;
        public int QuantityAvailable { get; set; } = -1;
        public bool RequiresParentApproval { get; set; } = true;
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime? ExpiresAt { get; set; }
    }
    
    public class UpdateRedeemableItemDto : CreateRedeemableItemDto
    {
        public bool IsActive { get; set; }
    }
}