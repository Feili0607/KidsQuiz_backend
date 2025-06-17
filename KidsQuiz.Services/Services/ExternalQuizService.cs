using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Data.ValueObjects;
using KidsQuiz.Services.Interfaces;

namespace KidsQuiz.Services.Services
{
    public class ExternalQuizService : IExternalQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly IQuestionBankService _questionBankService;

        public ExternalQuizService(HttpClient httpClient, IQuestionBankService questionBankService)
        {
            _httpClient = httpClient;
            _questionBankService = questionBankService;
        }

        public async Task<IEnumerable<QuestionBank>> FetchQuestionsFromOpenTDBAsync(
            int count = 10,
            string category = null,
            string difficulty = null)
        {
            var url = $"https://opentdb.com/api.php?amount={count}";
            if (!string.IsNullOrEmpty(category))
                url += $"&category={category}";
            if (!string.IsNullOrEmpty(difficulty))
                url += $"&difficulty={difficulty.ToLower()}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenTDBResponse>(content);

            if (result?.Results == null)
                return new List<QuestionBank>();

            var questions = new List<QuestionBank>();
            foreach (var q in result.Results)
            {
                var question = new QuestionBank
                {
                    Text = q.Question,
                    Options = new List<string>(q.IncorrectAnswers) { q.CorrectAnswer },
                    CorrectAnswerIndex = q.IncorrectAnswers.Count, // Changed from Length to Count
                    Explanation = "Correct answer: " + q.CorrectAnswer,
                    DifficultyLevel = MapDifficultyLevel(q.Difficulty),
                    Points = CalculatePoints(q.Difficulty),
                    TargetAgeGroup = MapCategoryToAgeGroup(q.Category),
                    Category = MapCategoryToInterestCategory(q.Category),
                    Tags = new List<string> { q.Category, q.Difficulty },
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0,
                    SuccessRate = 0,
                    ImageUrl = "",
                    AudioUrl = ""
                };

                // Shuffle options
                var random = new Random();
                var options = new List<string>(question.Options);
                for (int i = options.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    string temp = options[i];
                    options[i] = options[j];
                    options[j] = temp;
                }
                question.Options = options;
                question.CorrectAnswerIndex = options.IndexOf(q.CorrectAnswer);

                questions.Add(question);
            }

            return questions;
        }

        private DifficultyLevel MapDifficultyLevel(string difficulty)
        {
            return difficulty?.ToLower() switch
            {
                "easy" => DifficultyLevel.Beginner,
                "medium" => DifficultyLevel.Intermediate,
                "hard" => DifficultyLevel.Advanced,
                _ => DifficultyLevel.Beginner
            };
        }

        private int CalculatePoints(string difficulty)
        {
            return difficulty?.ToLower() switch
            {
                "easy" => 10,
                "medium" => 15,
                "hard" => 20,
                _ => 10
            };
        }

        private AgeGroup MapCategoryToAgeGroup(string category)
        {
            // This mapping can be customized based on your needs
            return AgeGroup.EarlyElementary; // Default to early elementary
        }

        private InterestCategory MapCategoryToInterestCategory(string category)
        {
            return category?.ToLower() switch
            {
                var c when c.Contains("science") => InterestCategory.Science,
                var c when c.Contains("history") => InterestCategory.History,
                var c when c.Contains("geography") => InterestCategory.Geography,
                var c when c.Contains("entertainment") => InterestCategory.Entertainment,
                var c when c.Contains("sports") => InterestCategory.Sports,
                var c when c.Contains("art") => InterestCategory.Art,
                var c when c.Contains("music") => InterestCategory.Music,
                _ => InterestCategory.General
            };
        }
    }

    public class OpenTDBResponse
    {
        public int ResponseCode { get; set; }
        public List<OpenTDBQuestion> Results { get; set; }
    }

    public class OpenTDBQuestion
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }
    }
} 