using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using KidsQuiz.Data.Models;
using System.Text.Json;

namespace KidsQuiz.Data.Configurations
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Title).IsRequired().HasMaxLength(200);
            builder.Property(q => q.Description).HasMaxLength(1000);
            builder.Property(q => q.CreatedAt).IsRequired();
            builder.Property(q => q.ModifiedAt);
            builder.Property(q => q.LLMPrompt).HasMaxLength(1000);
            builder.Property(q => q.EstimatedDurationMinutes).IsRequired();

            // Configure the Labels as a JSON array
            var property = builder.Property(q => q.Labels)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                );

            property.Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));

            // Add indexes
            builder.HasIndex(q => q.DifficultyLevel);
            builder.HasIndex(q => q.Rating);
            builder.HasIndex(q => q.CreatedAt);
        }
    }
} 