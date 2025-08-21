using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Services.DTOs.Rewards;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Services.Interfaces
{
    public interface IRewardService
    {
        // Wallet Management
        Task<RewardWalletDto> GetWalletAsync(int kidId);
        Task<RewardWalletDto> CreateWalletAsync(int kidId);
        Task<RewardWalletSummaryDto> GetWalletSummaryAsync(int kidId);
        Task<List<RewardWalletDto>> GetAllWalletsAsync();
        
        // Earning Rewards
        Task<RewardWalletDto> EarnRewardAsync(EarnRewardDto earnRewardDto);
        Task<RewardWalletDto> ProcessQuizCompletionRewardAsync(int kidId, int quizId, double scorePercentage);
        Task<RewardWalletDto> ProcessDailyLoginRewardAsync(int kidId);
        Task<RewardWalletDto> ProcessStreakRewardAsync(int kidId, int streakDays);
        Task<RewardWalletDto> ProcessAchievementRewardAsync(int kidId, string achievementType, string description);
        
        // Currency Conversion
        Task<RewardWalletDto> ConvertCurrencyAsync(ConvertCurrencyDto convertDto);
        Dictionary<string, decimal> GetConversionRates();
        
        // Transaction History
        Task<TransactionHistoryDto> GetTransactionHistoryAsync(TransactionFilterDto filter);
        Task<List<RewardTransactionDto>> GetRecentTransactionsAsync(int kidId, int count = 10);
        
        // Redeemable Items Management
        Task<List<RedeemableItemDto>> GetAvailableItemsAsync(int kidId);
        Task<RedeemableItemDto> GetRedeemableItemAsync(int id);
        Task<RedeemableItemDto> CreateRedeemableItemAsync(CreateRedeemableItemDto dto);
        Task<RedeemableItemDto> UpdateRedeemableItemAsync(int id, UpdateRedeemableItemDto dto);
        Task<bool> DeleteRedeemableItemAsync(int id);
        Task<List<RedeemableItemDto>> GetItemsByCategoryAsync(string category, int? kidId = null);
        
        // Redemptions
        Task<RedemptionDto> RequestRedemptionAsync(CreateRedemptionDto dto);
        Task<RedemptionDto> ApproveRedemptionAsync(ApproveRedemptionDto dto, string approverUserId);
        Task<RedemptionDto> FulfillRedemptionAsync(FulfillRedemptionDto dto);
        Task<RedemptionDto> CancelRedemptionAsync(int redemptionId, string reason);
        Task<List<RedemptionDto>> GetPendingRedemptionsAsync(int? kidId = null);
        Task<List<RedemptionDto>> GetRedemptionHistoryAsync(RedemptionFilterDto filter);
        
        // Level and Progress
        Task<int> CalculateExperienceForNextLevelAsync(int currentLevel);
        Task<RewardWalletDto> CheckAndProcessLevelUpAsync(int kidId);
        
        // Statistics
        Task<Dictionary<string, object>> GetRewardStatisticsAsync(int kidId);
        Task<Dictionary<string, int>> GetMonthlyEarningsAsync(int kidId, int year, int month);
    }
}