using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Rewards
{
    public class RewardWalletDto
    {
        public int Id { get; set; }
        public int KidId { get; set; }
        public string KidName { get; set; }
        
        // Current balances
        public int Coins { get; set; }
        public int SilverGems { get; set; }
        public int GoldCoins { get; set; }
        public int Rubies { get; set; }
        public int Sapphires { get; set; }
        public int Diamonds { get; set; }
        
        // Progress tracking
        public int TotalLifetimeCoins { get; set; }
        public int CurrentLevel { get; set; }
        public int ExperiencePoints { get; set; }
        public int ExperienceToNextLevel { get; set; }
        
        // Calculated total value in base coins
        public int TotalValueInCoins { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
    
    public class RewardWalletSummaryDto
    {
        public int KidId { get; set; }
        public Dictionary<string, int> Balances { get; set; }
        public int CurrentLevel { get; set; }
        public int TotalValueInCoins { get; set; }
    }
    
    public class EarnRewardDto
    {
        public int KidId { get; set; }
        public string RewardType { get; set; }  // Coins, SilverGems, etc.
        public int Amount { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDescription { get; set; }
        public int? RelatedActivityId { get; set; }
        public string Notes { get; set; }
    }
    
    public class ConvertCurrencyDto
    {
        public int KidId { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public int Amount { get; set; }
    }
}