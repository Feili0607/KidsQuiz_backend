using System;
using System.Collections.Generic;

namespace KidsQuiz.Data.Models
{
    public class RewardWallet
    {
        public int Id { get; set; }
        public int KidId { get; set; }
        public virtual Kid Kid { get; set; }
        
        // Different treasure types with increasing value
        public int Coins { get; set; } = 0;  // Basic currency
        public int SilverGems { get; set; } = 0;  // 10 coins = 1 silver gem
        public int GoldCoins { get; set; } = 0;  // 10 silver gems = 1 gold coin
        public int Rubies { get; set; } = 0;  // 5 gold coins = 1 ruby
        public int Sapphires { get; set; } = 0;  // 10 gold coins = 1 sapphire
        public int Diamonds { get; set; } = 0;  // 10 sapphires = 1 diamond
        
        public int TotalLifetimeCoins { get; set; } = 0;  // Track lifetime earnings
        public int CurrentLevel { get; set; } = 1;  // Reward level/tier
        public int ExperiencePoints { get; set; } = 0;  // XP for leveling up
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public virtual ICollection<RewardTransaction> Transactions { get; set; } = new List<RewardTransaction>();
        public virtual ICollection<Redemption> Redemptions { get; set; } = new List<Redemption>();
    }
}