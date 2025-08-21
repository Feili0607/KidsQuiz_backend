using System;
using System.Collections.Generic;

namespace KidsQuiz.Services.DTOs.Rewards
{
    public class RewardTransactionDto
    {
        public int Id { get; set; }
        public string RewardType { get; set; }
        public int Amount { get; set; }
        public string TransactionType { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDescription { get; set; }
        public int? RelatedActivityId { get; set; }
        public int BalanceAfterTransaction { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Notes { get; set; }
    }
    
    public class TransactionHistoryDto
    {
        public int TotalTransactions { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<RewardTransactionDto> Transactions { get; set; }
    }
    
    public class TransactionFilterDto
    {
        public int? KidId { get; set; }
        public string RewardType { get; set; }
        public string TransactionType { get; set; }
        public string ActivityType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}