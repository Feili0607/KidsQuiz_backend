using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using KidsQuiz.Data.Models;
using System.Text.Json;

namespace KidsQuiz.Data.Configurations
{
    public class QuestionBankConfiguration : IEntityTypeConfiguration<QuestionBank>
    {
        public void Configure(EntityTypeBuilder<QuestionBank> builder)
        {
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Text).IsRequired().HasMaxLength(1000);
            builder.Property(q => q.Explanation).HasMaxLength(1000);
            builder.Property(q => q.CreatedAt).IsRequired();
            builder.Property(q => q.ModifiedAt);
            builder.Property(q => q.IsActive).IsRequired();
            builder.Property(q => q.UsageCount).IsRequired();
            builder.Property(q => q.SuccessRate).IsRequired();

            // Configure the Options as a JSON array
            var optionsProperty = builder.Property(q => q.Options)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                );

            optionsProperty.Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

            // Configure the Tags as a JSON array
            var tagsProperty = builder.Property(q => q.Tags)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                );

            tagsProperty.Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

            // Add indexes
            builder.HasIndex(q => q.DifficultyLevel);
            builder.HasIndex(q => q.TargetAgeGroup);
            builder.HasIndex(q => q.Category);
            builder.HasIndex(q => q.IsActive);
            builder.HasIndex(q => q.SuccessRate);
        }
    }
} 