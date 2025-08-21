using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Data.Configurations
{
    public class RewardTransactionConfiguration : IEntityTypeConfiguration<RewardTransaction>
    {
        public void Configure(EntityTypeBuilder<RewardTransaction> builder)
        {
            builder.ToTable("RewardTransactions");
            
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.RewardType)
                .IsRequired()
                .HasConversion<int>();
            
            builder.Property(t => t.Amount).IsRequired();
            
            builder.Property(t => t.TransactionType)
                .IsRequired()
                .HasConversion<int>();
            
            builder.Property(t => t.ActivityType)
                .IsRequired()
                .HasConversion<int>();
            
            builder.Property(t => t.ActivityDescription)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(t => t.Notes).HasMaxLength(1000);
            
            builder.Property(t => t.TransactionDate).IsRequired();
            builder.Property(t => t.BalanceAfterTransaction).IsRequired();
            
            // Relationships
            builder.HasOne(t => t.RewardWallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.RewardWalletId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(t => t.RewardWalletId);
            builder.HasIndex(t => t.TransactionDate);
            builder.HasIndex(t => t.ActivityType);
            builder.HasIndex(t => new { t.RewardWalletId, t.TransactionDate }).HasDatabaseName("IX_RewardTransaction_Wallet_Date");
        }
    }
}