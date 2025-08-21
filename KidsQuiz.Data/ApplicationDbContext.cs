using Microsoft.EntityFrameworkCore;
using KidsQuiz.Data.Models;
using KidsQuiz.Data.Configurations;
using KidsQuiz.Data.ValueObjects;

namespace KidsQuiz.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kid> Kids { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizSolvingRecord> QuizSolvingRecords { get; set; }
        public DbSet<QuestionBank> QuestionBanks { get; set; }
        
        // Reward System
        public DbSet<RewardWallet> RewardWallets { get; set; }
        public DbSet<RewardTransaction> RewardTransactions { get; set; }
        public DbSet<RedeemableItem> RedeemableItems { get; set; }
        public DbSet<Redemption> Redemptions { get; set; }
        
        // User Management
        public DbSet<User> Users { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<KidGuardianRelationship> KidGuardianRelationships { get; set; }
        public DbSet<ParentKidRelationship> ParentKidRelationships { get; set; }
        public DbSet<TeacherKidRelationship> TeacherKidRelationships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new KidConfiguration());
            modelBuilder.ApplyConfiguration(new QuizConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new QuizSolvingRecordConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionBankConfiguration());
            
            // Reward System Configurations
            modelBuilder.ApplyConfiguration(new RewardWalletConfiguration());
            modelBuilder.ApplyConfiguration(new RewardTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new RedeemableItemConfiguration());
            modelBuilder.ApplyConfiguration(new RedemptionConfiguration());
            
            // User Management Configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new GuardianConfiguration());
            modelBuilder.ApplyConfiguration(new KidGuardianRelationshipConfiguration());
            modelBuilder.ApplyConfiguration(new ParentKidRelationshipConfiguration());
            modelBuilder.ApplyConfiguration(new TeacherKidRelationshipConfiguration());

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Math Quiz for Early Elementary
            var mathQuiz = new Quiz
            {
                Id = 1,
                Title = "Fun Math Quiz for Early Elementary",
                Description = "A fun quiz to test your basic math skills! Perfect for kids in grades 1-3.",
                Content = "Basic arithmetic operations for young learners",
                DifficultyLevel = 0, // Beginner
                Rating = 0,
                RatingCount = 0,
                CreatedAt = DateTime.UtcNow,
                Labels = new List<string> { "math", "arithmetic", "beginner", "early-elementary" },
                EstimatedDurationMinutes = 10,
                IsGeneratedByLLM = false,
                LLMPrompt = ""
            };

            modelBuilder.Entity<Quiz>().HasData(mathQuiz);

            // Add questions for the math quiz
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    Id = 1,
                    QuizId = 1,
                    Text = "What is 2 + 3?",
                    Options = new List<string> { "4", "5", "6", "7" },
                    CorrectAnswerIndex = 1,
                    Explanation = "2 + 3 = 5",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                },
                new Question
                {
                    Id = 2,
                    QuizId = 1,
                    Text = "What is 5 - 2?",
                    Options = new List<string> { "2", "3", "4", "5" },
                    CorrectAnswerIndex = 1,
                    Explanation = "5 - 2 = 3",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                },
                new Question
                {
                    Id = 3,
                    QuizId = 1,
                    Text = "What is 4 × 2?",
                    Options = new List<string> { "6", "7", "8", "9" },
                    CorrectAnswerIndex = 2,
                    Explanation = "4 × 2 = 8",
                    DifficultyLevel = DifficultyLevel.Intermediate,
                    Points = 15,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Science Quiz for Early Elementary
            var scienceQuiz = new Quiz
            {
                Id = 2,
                Title = "Amazing Science Facts for Kids",
                Description = "Test your knowledge about the world around us! Perfect for curious young minds.",
                Content = "Basic science concepts for curious minds",
                DifficultyLevel = 1, // Intermediate
                Rating = 0,
                RatingCount = 0,
                CreatedAt = DateTime.UtcNow,
                Labels = new List<string> { "science", "nature", "facts", "early-elementary" },
                EstimatedDurationMinutes = 15,
                IsGeneratedByLLM = false,
                LLMPrompt = ""
            };

            modelBuilder.Entity<Quiz>().HasData(scienceQuiz);

            // Add questions for the science quiz
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    Id = 4,
                    QuizId = 2,
                    Text = "What do plants need to grow?",
                    Options = new List<string> { "Water and sunlight", "Only water", "Only sunlight", "Neither water nor sunlight" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Plants need both water and sunlight to grow properly",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                },
                new Question
                {
                    Id = 5,
                    QuizId = 2,
                    Text = "What is the closest planet to the Sun?",
                    Options = new List<string> { "Venus", "Mercury", "Mars", "Earth" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Mercury is the closest planet to the Sun in our solar system",
                    DifficultyLevel = DifficultyLevel.Intermediate,
                    Points = 15,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // English Quiz for Early Elementary
            var englishQuiz = new Quiz
            {
                Id = 3,
                Title = "Fun with English Words",
                Description = "Learn and practice basic English vocabulary and grammar!",
                Content = "Basic English language skills for young learners",
                DifficultyLevel = 0, // Beginner
                Rating = 0,
                RatingCount = 0,
                CreatedAt = DateTime.UtcNow,
                Labels = new List<string> { "english", "vocabulary", "grammar", "early-elementary" },
                EstimatedDurationMinutes = 12,
                IsGeneratedByLLM = false,
                LLMPrompt = ""
            };

            modelBuilder.Entity<Quiz>().HasData(englishQuiz);

            // Add questions for the English quiz
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    Id = 6,
                    QuizId = 3,
                    Text = "Which word is a noun?",
                    Options = new List<string> { "Run", "Happy", "Dog", "Quickly" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A noun is a person, place, or thing. 'Dog' is a thing, so it's a noun.",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                },
                new Question
                {
                    Id = 7,
                    QuizId = 3,
                    Text = "What is the opposite of 'hot'?",
                    Options = new List<string> { "Warm", "Cold", "Cool", "Wet" },
                    CorrectAnswerIndex = 1,
                    Explanation = "The opposite of 'hot' is 'cold'",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                },
                new Question
                {
                    Id = 8,
                    QuizId = 3,
                    Text = "Which sentence is correct?",
                    Options = new List<string> { 
                        "I am going to the park", 
                        "I going to the park", 
                        "I goes to the park", 
                        "I go to the park yesterday" 
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "The correct sentence is 'I am going to the park' because it uses the correct present continuous tense.",
                    DifficultyLevel = DifficultyLevel.Intermediate,
                    Points = 15,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Math Quiz for Late Elementary
            var advancedMathQuiz = new Quiz
            {
                Id = 4,
                Title = "Math Challenge for Late Elementary",
                Description = "Test your math skills with these challenging problems!",
                Content = "Advanced arithmetic and basic algebra for grades 4-6",
                DifficultyLevel = 2, // Advanced
                Rating = 0,
                RatingCount = 0,
                CreatedAt = DateTime.UtcNow,
                Labels = new List<string> { "math", "arithmetic", "algebra", "late-elementary" },
                EstimatedDurationMinutes = 20,
                IsGeneratedByLLM = false,
                LLMPrompt = ""
            };

            modelBuilder.Entity<Quiz>().HasData(advancedMathQuiz);

            // Add questions for the advanced math quiz
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    Id = 9,
                    QuizId = 4,
                    Text = "What is 12 × 8?",
                    Options = new List<string> { "86", "96", "106", "116" },
                    CorrectAnswerIndex = 1,
                    Explanation = "12 × 8 = 96",
                    DifficultyLevel = DifficultyLevel.Intermediate,
                    Points = 15,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                },
                new Question
                {
                    Id = 10,
                    QuizId = 4,
                    Text = "If x + 5 = 12, what is x?",
                    Options = new List<string> { "5", "6", "7", "8" },
                    CorrectAnswerIndex = 2,
                    Explanation = "x + 5 = 12, so x = 12 - 5 = 7",
                    DifficultyLevel = DifficultyLevel.Advanced,
                    Points = 20,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                },
                new Question
                {
                    Id = 11,
                    QuizId = 4,
                    Text = "What is 144 ÷ 12?",
                    Options = new List<string> { "10", "11", "12", "13" },
                    CorrectAnswerIndex = 2,
                    Explanation = "144 ÷ 12 = 12",
                    DifficultyLevel = DifficultyLevel.Intermediate,
                    Points = 15,
                    ImageUrl = "",
                    AudioUrl = "",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed QuestionBank data
            var mathQuestions = new[]
            {
                new QuestionBank
                {
                    Id = 1,
                    Text = "What is 2 + 3?",
                    Options = new List<string> { "4", "5", "6", "7" },
                    CorrectAnswerIndex = 1,
                    Explanation = "2 + 3 = 5",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    TargetAgeGroup = AgeGroup.EarlyElementary,
                    Category = InterestCategory.Math,
                    Tags = new List<string> { "addition", "basic-math" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                },
                new QuestionBank
                {
                    Id = 2,
                    Text = "What is 5 - 2?",
                    Options = new List<string> { "2", "3", "4", "5" },
                    CorrectAnswerIndex = 1,
                    Explanation = "5 - 2 = 3",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    TargetAgeGroup = AgeGroup.EarlyElementary,
                    Category = InterestCategory.Math,
                    Tags = new List<string> { "subtraction", "basic-math" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                },
                new QuestionBank
                {
                    Id = 3,
                    Text = "What is 4 × 2?",
                    Options = new List<string> { "6", "7", "8", "9" },
                    CorrectAnswerIndex = 2,
                    Explanation = "4 × 2 = 8",
                    DifficultyLevel = DifficultyLevel.Intermediate,
                    Points = 15,
                    TargetAgeGroup = AgeGroup.EarlyElementary,
                    Category = InterestCategory.Math,
                    Tags = new List<string> { "multiplication", "basic-math" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                }
            };

            var scienceQuestions = new[]
            {
                new QuestionBank
                {
                    Id = 4,
                    Text = "What do plants need to grow?",
                    Options = new List<string> { "Water and sunlight", "Only water", "Only sunlight", "Neither water nor sunlight" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Plants need both water and sunlight to grow properly",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    TargetAgeGroup = AgeGroup.EarlyElementary,
                    Category = InterestCategory.Science,
                    Tags = new List<string> { "plants", "biology" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                },
                new QuestionBank
                {
                    Id = 5,
                    Text = "What is the closest planet to the Sun?",
                    Options = new List<string> { "Venus", "Mercury", "Mars", "Earth" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Mercury is the closest planet to the Sun in our solar system",
                    DifficultyLevel = DifficultyLevel.Intermediate,
                    Points = 15,
                    TargetAgeGroup = AgeGroup.EarlyElementary,
                    Category = InterestCategory.Science,
                    Tags = new List<string> { "planets", "solar-system" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                }
            };

            var englishQuestions = new[]
            {
                new QuestionBank
                {
                    Id = 6,
                    Text = "Which word is a noun?",
                    Options = new List<string> { "Run", "Happy", "Dog", "Quickly" },
                    CorrectAnswerIndex = 2,
                    Explanation = "A noun is a person, place, or thing. 'Dog' is a thing, so it's a noun.",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    TargetAgeGroup = AgeGroup.EarlyElementary,
                    Category = InterestCategory.Literature,
                    Tags = new List<string> { "grammar", "parts-of-speech" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                },
                new QuestionBank
                {
                    Id = 7,
                    Text = "What is the opposite of 'hot'?",
                    Options = new List<string> { "Warm", "Cold", "Cool", "Wet" },
                    CorrectAnswerIndex = 1,
                    Explanation = "The opposite of 'hot' is 'cold'",
                    DifficultyLevel = DifficultyLevel.Beginner,
                    Points = 10,
                    TargetAgeGroup = AgeGroup.EarlyElementary,
                    Category = InterestCategory.Literature,
                    Tags = new List<string> { "vocabulary", "antonyms" },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                }
            };

            modelBuilder.Entity<QuestionBank>().HasData(mathQuestions);
            modelBuilder.Entity<QuestionBank>().HasData(scienceQuestions);
            modelBuilder.Entity<QuestionBank>().HasData(englishQuestions);
        }
    }
} 