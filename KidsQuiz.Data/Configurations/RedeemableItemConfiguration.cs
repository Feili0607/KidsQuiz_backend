using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Data.Configurations
{
    public class RedeemableItemConfiguration : IEntityTypeConfiguration<RedeemableItem>
    {
        public void Configure(EntityTypeBuilder<RedeemableItem> builder)
        {
            builder.ToTable("RedeemableItems");
            
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(1000);
            
            builder.Property(r => r.Category)
                .IsRequired()
                .HasConversion<int>();
            
            builder.Property(r => r.MinimumLevel)
                .IsRequired()
                .HasDefaultValue(1);
            
            builder.Property(r => r.QuantityAvailable)
                .IsRequired()
                .HasDefaultValue(-1);
            
            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(r => r.RequiresParentApproval)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(r => r.ImageUrl).HasMaxLength(500);
            
            builder.Property(r => r.SortOrder).HasDefaultValue(0);
            
            builder.Property(r => r.CreatedAt).IsRequired();
            
            // Relationships
            builder.HasMany(r => r.Redemptions)
                .WithOne(red => red.RedeemableItem)
                .HasForeignKey(red => red.RedeemableItemId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indexes
            builder.HasIndex(r => r.Category);
            builder.HasIndex(r => r.IsActive);
            builder.HasIndex(r => r.MinimumLevel);
            builder.HasIndex(r => new { r.IsActive, r.Category, r.SortOrder });
        }
    }
}