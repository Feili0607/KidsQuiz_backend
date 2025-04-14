using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Entities;

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
            builder.Property(q => q.Labels)
                .HasColumnType("jsonb");

            // Add indexes
            builder.HasIndex(q => q.TargetAgeGroup);
            builder.HasIndex(q => q.DifficultyLevel);
            builder.HasIndex(q => q.Category);
            builder.HasIndex(q => q.Rating);
            builder.HasIndex(q => q.CreatedAt);
        }
    }
} 