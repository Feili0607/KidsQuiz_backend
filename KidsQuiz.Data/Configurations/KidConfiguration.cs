using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using KidsQuiz.Data.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KidsQuiz.Data.Configurations
{
    public class KidConfiguration : IEntityTypeConfiguration<Kid>
    {
        public void Configure(EntityTypeBuilder<Kid> builder)
        {
            builder.ToTable("Kids");
            
            builder.HasKey(k => k.Id);
            
            // Basic Information
            builder.Property(k => k.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(k => k.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(k => k.PreferredName)
                .HasMaxLength(100);
            
            builder.Property(k => k.DateOfBirth)
                .IsRequired();
            
            builder.Property(k => k.Gender)
                .IsRequired()
                .HasConversion<int>();
            
            // Account Information
            builder.Property(k => k.Username)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(k => k.Email)
                .HasMaxLength(256);
            
            builder.Property(k => k.AvatarUrl)
                .HasMaxLength(500);
            
            // Educational Information
            builder.Property(k => k.Grade)
                .HasMaxLength(20);
            
            builder.Property(k => k.School)
                .HasMaxLength(200);
            
            builder.Property(k => k.TeacherId)
                .HasMaxLength(128);
            
            // Personal Details
            builder.Property(k => k.Intro)
                .HasMaxLength(1000);
            
            // Lists as JSON
            builder.Property(k => k.Interests)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)");
            
            builder.Property(k => k.FavoriteSubjects)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)");
            
            // Dynamic Properties
            var dynamicPropertiesProperty = builder.Property(k => k.DynamicProperties)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>()
                );
            
            dynamicPropertiesProperty.Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
                c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            ));
            
            // Preferences
            builder.Property(k => k.Preferences)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<KidPreferences>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)");
            
            builder.Property(k => k.TimeZone)
                .HasMaxLength(50)
                .HasDefaultValue("UTC");
            
            builder.Property(k => k.Language)
                .HasMaxLength(10)
                .HasDefaultValue("en");
            
            // Account Status
            builder.Property(k => k.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            builder.Property(k => k.CreatedAt)
                .IsRequired();
            
            builder.Property(k => k.UpdatedAt)
                .IsRequired();
            
            // Computed properties - ignore in database
            builder.Ignore(k => k.FullName);
            builder.Ignore(k => k.Age);
            builder.Ignore(k => k.PrimaryGuardian);
            builder.Ignore(k => k.GuardianCount);
            
            // Relationships
            builder.HasMany(k => k.GuardianRelationships)
                .WithOne(gr => gr.Kid)
                .HasForeignKey(gr => gr.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(k => k.TeacherRelationships)
                .WithOne(tr => tr.Kid)
                .HasForeignKey(tr => tr.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(k => k.QuizSolvingRecords)
                .WithOne(qsr => qsr.Kid)
                .HasForeignKey(qsr => qsr.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(k => k.Quizzes)
                .WithOne(q => q.Kid)
                .HasForeignKey(q => q.KidId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.HasOne(k => k.RewardWallet)
                .WithOne(w => w.Kid)
                .HasForeignKey<RewardWallet>(w => w.KidId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            builder.HasIndex(k => k.Username).IsUnique();
            builder.HasIndex(k => k.Email);
            builder.HasIndex(k => new { k.IsActive, k.Grade });
            builder.HasIndex(k => k.LastActiveAt);
        }
    }
}