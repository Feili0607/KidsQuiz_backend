using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Data.Configurations
{
    public class RedemptionConfiguration : IEntityTypeConfiguration<Redemption>
    {
        public void Configure(EntityTypeBuilder<Redemption> builder)
        {
            builder.ToTable("Redemptions");
            
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<int>();
            
            builder.Property(r => r.RequestedAt).IsRequired();
            
            builder.Property(r => r.ApprovedBy).HasMaxLength(128);
            
            builder.Property(r => r.Notes).HasMaxLength(1000);
            builder.Property(r => r.RejectionReason).HasMaxLength(500);
            
            // Relationships
            builder.HasOne(r => r.RewardWallet)
                .WithMany(w => w.Redemptions)
                .HasForeignKey(r => r.RewardWalletId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(r => r.RedeemableItem)
                .WithMany(i => i.Redemptions)
                .HasForeignKey(r => r.RedeemableItemId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indexes
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.RequestedAt);
            builder.HasIndex(r => new { r.RewardWalletId, r.Status });
            builder.HasIndex(r => new { r.ApprovedBy, r.Status });
        }
    }
}