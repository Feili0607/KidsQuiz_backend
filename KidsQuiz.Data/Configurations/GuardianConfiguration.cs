using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;
using System.Text.Json;

namespace KidsQuiz.Data.Configurations
{
    public class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
    {
        public void Configure(EntityTypeBuilder<Guardian> builder)
        {
            builder.ToTable("Guardians");
            
            builder.HasKey(g => g.Id);
            
            // Azure AD Integration
            builder.Property(g => g.AzureAdObjectId)
                .IsRequired()
                .HasMaxLength(128);
            
            // Personal Information
            builder.Property(g => g.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(g => g.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(g => g.Email)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.Property(g => g.PhoneNumber)
                .HasMaxLength(20);
            
            builder.Property(g => g.AlternatePhoneNumber)
                .HasMaxLength(20);
            
            // Guardian Type
            builder.Property(g => g.Type)
                .IsRequired()
                .HasConversion<int>();
            
            builder.Property(g => g.RelationshipType)
                .IsRequired()
                .HasConversion<int>();
            
            // Address Information
            builder.Property(g => g.Address)
                .HasMaxLength(500);
            
            builder.Property(g => g.City)
                .HasMaxLength(100);
            
            builder.Property(g => g.State)
                .HasMaxLength(50);
            
            builder.Property(g => g.PostalCode)
                .HasMaxLength(20);
            
            builder.Property(g => g.Country)
                .HasMaxLength(100);
            
            // Preferences
            builder.Property(g => g.NotificationPreferences)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<NotificationPreferences>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");
            
            builder.Property(g => g.PreferredLanguage)
                .HasMaxLength(10)
                .HasDefaultValue("en");
            
            builder.Property(g => g.TimeZone)
                .HasMaxLength(50)
                .HasDefaultValue("UTC");
            
            // Status
            builder.Property(g => g.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(g => g.EmailVerified)
                .IsRequired()
                .HasDefaultValue(false);
            
            builder.Property(g => g.PhoneVerified)
                .IsRequired()
                .HasDefaultValue(false);
            
            // Timestamps
            builder.Property(g => g.CreatedAt)
                .IsRequired();
            
            builder.Property(g => g.UpdatedAt)
                .IsRequired();
            
            // Relationships
            builder.HasMany(g => g.KidRelationships)
                .WithOne(kr => kr.Guardian)
                .HasForeignKey(kr => kr.GuardianId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(g => g.AzureAdObjectId).IsUnique();
            builder.HasIndex(g => g.Email);
            builder.HasIndex(g => g.PhoneNumber);
            builder.HasIndex(g => new { g.IsActive, g.Type });
        }
    }
}