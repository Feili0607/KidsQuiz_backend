using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;

namespace KidsQuiz.Data.Configurations
{
    public class QuizSolvingRecordConfiguration : IEntityTypeConfiguration<QuizSolvingRecord>
    {
        public void Configure(EntityTypeBuilder<QuizSolvingRecord> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.StartedAt).IsRequired();
            builder.Property(r => r.Score).IsRequired();
            builder.Property(r => r.TimeTakenInSeconds).IsRequired();

            // Configure relationships
            builder.HasOne(r => r.Kid)
                .WithMany(k => k.QuizSolvingRecords)
                .HasForeignKey(r => r.KidId);

            builder.HasOne(r => r.Quiz)
                .WithMany(q => q.QuizSolvingRecords)
                .HasForeignKey(r => r.QuizId);
        }
    }
} 