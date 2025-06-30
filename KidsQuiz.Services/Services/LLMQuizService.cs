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
            var prompt = @$"Generate a fun and educational quiz for a child named {userInfo.Name} who is in {userInfo.Grade}.
The child's interests are: '{userInfo.Intro}'.

Create a quiz with:
- A fun, engaging title.
- A short, encouraging description.
- 5 multiple-choice questions appropriate for {userInfo.Grade}.
- Questions should be related to the child's interests: {userInfo.Intro}.
- Each question must have 4 options.
- Indicate the correct answer index (0-3).
- Provide a brief, simple explanation for the correct answer.

Return the result as a single JSON object with the following structure:
{{
  ""title"": ""string"",
  ""description"": ""string"",
  ""questions"": [
    {{
      ""text"": ""string"",
      ""options"": [""string"", ""string"", ""string"", ""string""],
      ""correctAnswerIndex"": 0,
      ""explanation"": ""string""
    }}
  ]
}}";

            var requestBody = new
            {
                model = _openAiModel,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful quiz generator for children." },
                    new { role = "user", content = prompt }
                },
                response_format = new { type = "json_object" }, // Enforce JSON output
                max_tokens = 2000,
                temperature = 0.8
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Consider logging the error content for debugging
                throw new Exception($"OpenAI API call failed with status code {response.StatusCode}: {errorContent}");
            }
            
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content").GetString();

            // The content is a JSON string, so we deserialize it into our Quiz and Question models
            var quizData = JsonSerializer.Deserialize<QuizGenerationDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var quiz = new Quiz
            {
                Title = quizData.Title,
                Description = quizData.Description,
                Content = $"Quiz generated for {userInfo.Name} based on their interests.",
                DifficultyLevel = (int)GetDifficultyFromGrade(userInfo.Grade),
                IsGeneratedByLLM = true,
                LLMPrompt = prompt,
                EstimatedDurationMinutes = 10,
                CreatedAt = DateTime.UtcNow,
                Questions = quizData.Questions.Select(q => new Question
                {
                    Text = q.Text,
                    Options = q.Options,
                    CorrectAnswerIndex = q.CorrectAnswerIndex,
                    Explanation = q.Explanation,
                    DifficultyLevel = (Data.ValueObjects.DifficultyLevel)GetDifficultyFromGrade(userInfo.Grade), // Inherit difficulty
                    Points = 10, // Default points
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            return quiz;
        }

        // This method is no longer the primary one for your goal, but we leave it for now.
        public async Task<QuestionBank> GenerateQuestionAsync(string subject, string grade, string difficulty)
        {
            var prompt = @$"Generate a multiple-choice {subject} question for a student in grade {grade} at {difficulty} difficulty.
Return the result as a JSON object with the following fields:
- question: string
- options: array of 4 strings
- correct_answer_index: integer (0-based)
- explanation: string
Example:
{{
  ""question"": ""What is 2 + 2?"",
  ""options"": [""3"", ""4"", ""5"", ""6""],
  ""correct_answer_index"": 1,
  ""explanation"": ""2 + 2 = 4""
}}";

            var requestBody = new
            {
                model = _openAiModel,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful quiz generator for children." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 400,
                temperature = 0.7
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content").GetString();

            // Parse the JSON from the LLM's response
            var questionJson = JsonDocument.Parse(content);
            var root = questionJson.RootElement;

            var question = new QuestionBank
            {
                Text = root.GetProperty("question").GetString(),
                Options = new List<string>(),
                CorrectAnswerIndex = root.GetProperty("correct_answer_index").GetInt32(),
                Explanation = root.GetProperty("explanation").GetString(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                UsageCount = 0,
                SuccessRate = 0,
                ImageUrl = "",
                AudioUrl = ""
            };
            foreach (var opt in root.GetProperty("options").EnumerateArray())
            {
                question.Options.Add(opt.GetString());
            }
            return question;
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