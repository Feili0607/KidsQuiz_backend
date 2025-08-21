using System;

namespace KidsQuiz.Data.Models
{
    public class RewardTransaction
    {
        public int Id { get; set; }
        public int RewardWalletId { get; set; }
        public virtual RewardWallet RewardWallet { get; set; }
        
        public RewardType RewardType { get; set; }
        public int Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public ActivityType ActivityType { get; set; }
        public string ActivityDescription { get; set; }
        public int? RelatedActivityId { get; set; }  // QuizId, TaskId, etc.
        
        public int BalanceAfterTransaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
    }
    
    public enum RewardType
    {
        Coins = 0,
        SilverGems = 1,
        GoldCoins = 2,
        Rubies = 3,
        Sapphires = 4,
        Diamonds = 5
    }
    
    public enum TransactionType
    {
        Earned = 0,
        Spent = 1,
        Bonus = 2,
        Penalty = 3,
        Converted = 4,  // When converting between currencies
        Expired = 5
    }
    
    public enum ActivityType
    {
        QuizCompleted = 0,
        QuizPerfectScore = 1,
        DailyLogin = 2,
        WeeklyStreak = 3,
        LevelUp = 4,
        Achievement = 5,
        Challenge = 6,
        HomeworkCompleted = 7,
        ReadingTime = 8,
        CreativeActivity = 9,
        Redemption = 10,
        ParentBonus = 11,
        SpecialEvent = 12
    }
}