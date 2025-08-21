using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Data.Configurations
{
    public class RewardWalletConfiguration : IEntityTypeConfiguration<RewardWallet>
    {
        public void Configure(EntityTypeBuilder<RewardWallet> builder)
        {
            builder.ToTable("RewardWallets");
            
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Coins).IsRequired().HasDefaultValue(0);
            builder.Property(r => r.SilverGems).IsRequired().HasDefaultValue(0);
            builder.Property(r => r.GoldCoins).IsRequired().HasDefaultValue(0);
            builder.Property(r => r.Rubies).IsRequired().HasDefaultValue(0);
            builder.Property(r => r.Sapphires).IsRequired().HasDefaultValue(0);
            builder.Property(r => r.Diamonds).IsRequired().HasDefaultValue(0);
            builder.Property(r => r.TotalLifetimeCoins).IsRequired().HasDefaultValue(0);
            builder.Property(r => r.CurrentLevel).IsRequired().HasDefaultValue(1);
            builder.Property(r => r.ExperiencePoints).IsRequired().HasDefaultValue(0);
            
            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt).IsRequired();
            
            // One-to-one relationship with Kid
            builder.HasOne(r => r.Kid)
                .WithOne(k => k.RewardWallet)
                .HasForeignKey<RewardWallet>(r => r.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // One-to-many relationships
            builder.HasMany(r => r.Transactions)
                .WithOne(t => t.RewardWallet)
                .HasForeignKey(t => t.RewardWalletId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(r => r.Redemptions)
                .WithOne(red => red.RewardWallet)
                .HasForeignKey(red => red.RewardWalletId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indexes
            builder.HasIndex(r => r.KidId).IsUnique();
            builder.HasIndex(r => r.CurrentLevel);
        }
    }
}