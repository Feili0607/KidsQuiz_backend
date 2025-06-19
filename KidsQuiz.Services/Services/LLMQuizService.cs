using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using System;
using System.Collections.Generic;

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
    }
} 