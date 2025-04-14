using Microsoft.EntityFrameworkCore;
using KidsQuiz.Data.Entities;
using KidsQuiz.Data.Configurations;

namespace KidsQuiz.Data.DbContexts
{
    public class KidsQuizDbContext : DbContext
    {
        public KidsQuizDbContext(DbContextOptions<KidsQuizDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kid> Kids { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizSolvingRecord> QuizSolvingRecords { get; set; }
        public DbSet<AnswerRecord> AnswerRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new KidConfiguration());
            modelBuilder.ApplyConfiguration(new QuizConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new QuizSolvingRecordConfiguration());
            modelBuilder.ApplyConfiguration(new AnswerRecordConfiguration());
        }
    }
} 