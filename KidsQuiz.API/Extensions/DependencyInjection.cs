using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.Services;
using KidsQuiz.Data;
using Microsoft.EntityFrameworkCore;

namespace KidsQuiz.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add Services
            services.AddScoped<IKidService, KidService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IQuizSolvingRecordService, QuizSolvingRecordService>();
            services.AddScoped<IQuestionBankService, QuestionBankService>();

            return services;
        }
    }
} 