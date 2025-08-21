using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;
using System.Text.Json;

namespace KidsQuiz.Data.Configurations
{
    public class KidGuardianRelationshipConfiguration : IEntityTypeConfiguration<KidGuardianRelationship>
    {
        public void Configure(EntityTypeBuilder<KidGuardianRelationship> builder)
        {
            builder.ToTable("KidGuardianRelationships");
            
            builder.HasKey(r => r.Id);
            
            // Relationship Details
            builder.Property(r => r.IsPrimaryGuardian)
                .IsRequired()
                .HasDefaultValue(false);
            
            builder.Property(r => r.IsEmergencyContact)
                .IsRequired()
                .HasDefaultValue(false);
            
            builder.Property(r => r.Priority)
                .IsRequired()
                .HasDefaultValue(1);
            
            // Permissions
            builder.Property(r => r.Permissions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<GuardianPermissions>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            
            // Legal/Custody Information
            builder.Property(r => r.HasLegalCustody)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(r => r.HasEducationalRights)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(r => r.CustodyNotes)
                .HasMaxLength(1000);
            
            // Status
            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(RelationshipStatus.Active);
            
            builder.Property(r => r.InvitationToken)
                .HasMaxLength(256);
            
            builder.Property(r => r.DeactivationReason)
                .HasMaxLength(500);
            
            // Timestamps
            builder.Property(r => r.CreatedAt)
                .IsRequired();
            
            builder.Property(r => r.UpdatedAt)
                .IsRequired();
            
            // Relationships
            builder.HasOne(r => r.Kid)
                .WithMany(k => k.GuardianRelationships)
                .HasForeignKey(r => r.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(r => r.Guardian)
                .WithMany(g => g.KidRelationships)
                .HasForeignKey(r => r.GuardianId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(r => new { r.KidId, r.GuardianId }).IsUnique();
            builder.HasIndex(r => new { r.KidId, r.IsPrimaryGuardian });
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.InvitationToken);
            
            // Constraints - Ensure only 3 active guardians per kid (handled in service layer)
            // Ensure only one primary guardian per kid (handled in service layer)
        }
    }
}