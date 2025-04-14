using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services;
using KidsQuiz.Data.Repositories;
using KidsQuiz.Data.Interfaces;
using KidsQuiz.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using KidsQuiz.Services.Mappings;
using KidsQuiz.Services.Utilities;

namespace KidsQuiz.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<KidsQuizDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add AutoMapper
            services.AddAutoMapper(typeof(KidsQuizMappingProfile));

            // Add Repositories
            services.AddScoped<IKidRepository, KidRepository>();
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<IQuizSolvingRecordRepository, QuizSolvingRecordRepository>();

            // Add Services
            services.AddScoped<Services.Interfaces.IKidsService, KidsService>();
            services.AddScoped<Services.Interfaces.IQuizService, QuizService>();
            services.AddScoped<IQuizSolvingService, QuizSolvingService>();

            // Add Utilities
            services.AddScoped<IAgeGroupCalculator, AgeGroupCalculator>();
            services.AddScoped<IQuizDifficultyEvaluator, QuizDifficultyEvaluator>();

            return services;
        }
    }
} 