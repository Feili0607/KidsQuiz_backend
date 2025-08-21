using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.AzureAdObjectId)
                .IsRequired()
                .HasMaxLength(128);
            
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>();
            
            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            // One-to-one relationship with Kid (for Kid role users)
            builder.HasOne(u => u.Kid)
                .WithOne(k => k.User)
                .HasForeignKey<User>(u => u.KidId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Indexes
            builder.HasIndex(u => u.AzureAdObjectId).IsUnique();
            builder.HasIndex(u => u.Email);
            builder.HasIndex(u => u.Role);
        }
    }
    
    public class ParentKidRelationshipConfiguration : IEntityTypeConfiguration<ParentKidRelationship>
    {
        public void Configure(EntityTypeBuilder<ParentKidRelationship> builder)
        {
            builder.ToTable("ParentKidRelationships");
            
            builder.HasKey(p => p.Id);
            
            builder.HasOne(p => p.ParentUser)
                .WithMany(u => u.ParentKidRelationships)
                .HasForeignKey(p => p.ParentUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(p => p.Kid)
                .WithMany(k => k.ParentRelationships)
                .HasForeignKey(p => p.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Ensure unique parent-kid combination
            builder.HasIndex(p => new { p.ParentUserId, p.KidId }).IsUnique();
        }
    }
    
    public class TeacherKidRelationshipConfiguration : IEntityTypeConfiguration<TeacherKidRelationship>
    {
        public void Configure(EntityTypeBuilder<TeacherKidRelationship> builder)
        {
            builder.ToTable("TeacherKidRelationships");
            
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.ClassName).HasMaxLength(100);
            builder.Property(t => t.SchoolYear).HasMaxLength(20);
            
            builder.HasOne(t => t.TeacherUser)
                .WithMany(u => u.TeacherKidRelationships)
                .HasForeignKey(t => t.TeacherUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(t => t.Kid)
                .WithMany(k => k.TeacherRelationships)
                .HasForeignKey(t => t.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(t => new { t.TeacherUserId, t.KidId, t.SchoolYear });
        }
    }
}