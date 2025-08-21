using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class RedeemableItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RedeemableCategory Category { get; set; }
        
        // Cost in different currencies
        public int? CoinsCost { get; set; }
        public int? SilverGemsCost { get; set; }
        public int? GoldCoinsCost { get; set; }
        public int? RubiesCost { get; set; }
        public int? SapphiresCost { get; set; }
        public int? DiamondsCost { get; set; }
        
        public int MinimumLevel { get; set; } = 1;  // Minimum level required
        public int QuantityAvailable { get; set; } = -1;  // -1 means unlimited
        public bool IsActive { get; set; } = true;
        public bool RequiresParentApproval { get; set; } = true;
        
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        
        public virtual ICollection<Redemption> Redemptions { get; set; } = new List<Redemption>();
    }
    
    public enum RedeemableCategory
    {
        Toys = 0,
        ScreenTime = 1,
        Books = 2,
        Games = 3,
        Snacks = 4,
        Activities = 5,
        VirtualItems = 6,
        Privileges = 7,
        ExtraTime = 8,
        SpecialEvents = 9
    }
}