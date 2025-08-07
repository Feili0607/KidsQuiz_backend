using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using System;
using System.Collections.Generic;
using KidsQuiz.Services.DTOs.Quizzes;

namespace KidsQuiz.Services.Services
{
    public class LLMQuizService : ILLMQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;
        private readonly string _openAiModel;

        public LLMQuizService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            _openAiModel = "gpt-3.5-turbo"; // Or "gpt-4" if you have access
        }

        public async Task<Quiz> GenerateFullQuizAsync(UserInfoDto userInfo)
        {
            var subject = string.IsNullOrEmpty(userInfo.Subject) ? "General Knowledge" : userInfo.Subject;
            
            var prompt = @$"Generate a fun quiz for {userInfo.Name} (Grade {userInfo.Grade}) about {subject}.
Interests: {userInfo.Intro}

Create:
- Title mentioning {subject}
- Description about learning {subject}
- AT LEAST 10 multiple-choice questions about {subject}
- 4 options per question
- Correct answer index (0-3)
- Brief explanation

Return JSON: {{""title"":""string"",""description"":""string"",""questions"":[{{""text"":""string"",""options"":[""string"",""string"",""string"",""string""],""correctAnswerIndex"":0,""explanation"":""string""}}]}}";

            var requestBody = new
            {
                model = _openAiModel,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful quiz generator for children. Create questions specifically about the requested subject." },
                    new { role = "user", content = prompt }
                },
                response_format = new { type = "json_object" },
                max_tokens = 2000,
                temperature = 0.9
            };

            // Debug logging
            Console.WriteLine($"Generating quiz for subject: {subject}");
            Console.WriteLine($"User info: {userInfo.Name}, Grade: {userInfo.Grade}, Interests: {userInfo.Intro}");
            Console.WriteLine($"Prompt length: {prompt.Length} characters");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OpenAI API Error: {response.StatusCode} - {errorContent}");
                throw new Exception($"OpenAI API call failed with status code {response.StatusCode}: {errorContent}");
            }
            
            var responseString = await response.Content.ReadAsStringAsync();
            
            // Debug logging
            Console.WriteLine($"OpenAI Response received successfully");

            using var doc = JsonDocument.Parse(responseString);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content").GetString();

            // The content is a JSON string, so we deserialize it into our Quiz and Question models
            var quizData = JsonSerializer.Deserialize<QuizGenerationDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Debug logging
            Console.WriteLine($"Generated quiz title: {quizData.Title}");
            Console.WriteLine($"Number of questions: {quizData.Questions?.Count ?? 0}");

            var quiz = new Quiz
            {
                Title = quizData.Title ?? "Generated Quiz",
                Description = quizData.Description ?? "A fun quiz generated for you!",
                Content = $"Quiz generated for {userInfo.Name} based on their interests in {subject}.",
                DifficultyLevel = (int)GetDifficultyFromGrade(userInfo.Grade),
                IsGeneratedByLLM = true,
                LLMPrompt = $"Quiz for {userInfo.Name} about {subject}", // Store only a short version
                EstimatedDurationMinutes = 10,
                CreatedAt = DateTime.UtcNow,
                Rating = 0,
                RatingCount = 0,
                Labels = new List<string> { subject.ToLower() },
                Questions = quizData.Questions?.Select(q => new Question
                {
                    Text = q.Text ?? "Question",
                    Options = q.Options ?? new List<string> { "Option A", "Option B", "Option C", "Option D" },
                    CorrectAnswerIndex = q.CorrectAnswerIndex,
                    Explanation = q.Explanation ?? "This is the correct answer.",
                    DifficultyLevel = (Data.ValueObjects.DifficultyLevel)GetDifficultyFromGrade(userInfo.Grade),
                    Points = 10,
                    CreatedAt = DateTime.UtcNow,
                    AudioUrl = "",
                    ImageUrl = ""
                }).ToList() ?? new List<Question>()
            };

            return quiz;
        }

        private static Data.ValueObjects.DifficultyLevel GetDifficultyFromGrade(string grade)
        {
            // Simple logic to map grade to difficulty. This can be more sophisticated.
            if (grade.ToLower().Contains("k") || grade.Contains("1") || grade.Contains("2"))
                return Data.ValueObjects.DifficultyLevel.Beginner;
            if (grade.Contains("3") || grade.Contains("4") || grade.Contains("5"))
                return Data.ValueObjects.DifficultyLevel.Intermediate;
            return Data.ValueObjects.DifficultyLevel.Advanced;
        }
    }

    // Helper DTO for deserializing the quiz structure from OpenAI
    public class QuizGenerationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionGenerationDto> Questions { get; set; }
    }

    public class QuestionGenerationDto
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
    }
} 