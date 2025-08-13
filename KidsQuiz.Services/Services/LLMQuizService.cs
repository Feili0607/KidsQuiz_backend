using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Quizzes;
using Microsoft.Extensions.Logging;

namespace KidsQuiz.Services.Services
{
    public class LLMQuizService : ILLMQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;
        private readonly string _openAiModel;
        private readonly ILogger<LLMQuizService> _logger;

        public LLMQuizService(HttpClient httpClient, ILogger<LLMQuizService> logger)
        {
            _httpClient = httpClient;
            _openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            _openAiModel = "gpt-3.5-turbo"; // Or "gpt-4" if you have access
            _logger = logger;
        }

        public async Task<Quiz> GenerateFullQuizAsync(UserInfoDto userInfo)
        {
            var subject = string.IsNullOrEmpty(userInfo.Subject) ? "General Knowledge" : userInfo.Subject;
            
            // Add randomization to ensure variety
            var random = new Random();
            var quizThemes = new[] { "fun facts", "practical knowledge", "historical trivia", "real-world applications", "creative thinking", "problem solving", "amazing discoveries", "everyday science", "cool experiments", "fascinating stories" };
            var selectedTheme = quizThemes[random.Next(quizThemes.Length)];
            
            // Add more variety with time-based elements
            var currentHour = DateTime.UtcNow.Hour;
            var timeOfDay = currentHour < 12 ? "morning" : currentHour < 17 ? "afternoon" : "evening";
            var dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString().ToLower();
            
            // Add additional variety elements
            var quizStyles = new[] { "adventure", "challenge", "exploration", "journey", "quest", "discovery", "mission", "expedition", "investigation", "exploration" };
            var selectedStyle = quizStyles[random.Next(quizStyles.Length)];
            
            var prompt = @$"Generate a unique and diverse quiz for {userInfo.Name} (Grade {userInfo.Grade}) about {subject}.
Interests: {userInfo.Intro}
Theme Focus: {selectedTheme}
Time Context: {timeOfDay} on {dayOfWeek}
Style: {selectedStyle}

IMPORTANT: Make this quiz completely different from any other {subject} quiz. Use creative titles, varied question types, and diverse content.

CRITICAL: The CorrectAnswerIndex must be the ZERO-BASED index of the correct answer in the options array.
- If the correct answer is the FIRST option, CorrectAnswerIndex = 0
- If the correct answer is the SECOND option, CorrectAnswerIndex = 1  
- If the correct answer is the THIRD option, CorrectAnswerIndex = 2
- If the correct answer is the FOURTH option, CorrectAnswerIndex = 3

Create:
- A UNIQUE title that stands out from other {subject} quizzes (be creative and specific)
- A detailed description that explains what makes this quiz special
- AT LEAST 10 multiple-choice questions about {subject} with HIGH VARIETY:
  * Mix different aspects of {subject} (history, facts, practical applications, fun trivia)
  * Vary the question complexity and style
  * Include some unexpected or interesting angles
  * Make questions engaging and different from typical {subject} questions
- 4 unique options per question (avoid generic answers)
- CorrectAnswerIndex (0-3) - MUST match the position of the correct answer
- Brief but informative explanations

Return JSON: {{""title"":""string"",""description"":""string"",""questions"":[{{""text"":""string"",""options"":[""string"",""string"",""string"",""string""],""correctAnswerIndex"":0,""explanation"":""string""}}]}}

Make sure this quiz feels completely different and unique!
DOUBLE-CHECK that CorrectAnswerIndex matches the position of the correct answer in the options array!";

            var requestBody = new
            {
                model = _openAiModel,
                messages = new[]
                {
                    new { role = "system", content = "You are a creative quiz generator for children. Your job is to create UNIQUE and DIVERSE quizzes that are completely different from each other, even for the same subject. Always think outside the box and create engaging, varied content. Never repeat the same quiz structure or questions." },
                    new { role = "user", content = prompt }
                },
                response_format = new { type = "json_object" },
                max_tokens = 2000,
                temperature = 0.9
            };

            // Debug logging
            _logger.LogInformation("Generating quiz for subject: {Subject}", subject);
            _logger.LogInformation("User info: {Name}, Grade: {Grade}, Interests: {Interests}", userInfo.Name, userInfo.Grade, userInfo.Intro);
            _logger.LogInformation("Quiz variety elements - Theme: {Theme}, Time: {TimeOfDay}, Day: {DayOfWeek}, Style: {Style}", selectedTheme, timeOfDay, dayOfWeek, selectedStyle);
            _logger.LogInformation("Prompt length: {PromptLength} characters", prompt.Length);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("OpenAI API Error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                throw new Exception($"OpenAI API call failed with status code {response.StatusCode}: {errorContent}");
            }
            
            var responseString = await response.Content.ReadAsStringAsync();
            
            // Debug logging
            _logger.LogInformation("OpenAI Response received successfully");

            using var doc = JsonDocument.Parse(responseString);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content").GetString();

            // The content is a JSON string, so we deserialize it into our Quiz and Question models
            var quizData = JsonSerializer.Deserialize<QuizGenerationDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            // Debug logging
            _logger.LogInformation("Generated quiz title: {Title}", quizData.Title);
            _logger.LogInformation("Number of questions: {QuestionCount}", quizData.Questions?.Count ?? 0);

            // Validate and fix CorrectAnswerIndex values
            if (quizData.Questions != null)
            {
                foreach (var question in quizData.Questions)
                {
                    _logger.LogInformation("Validating question: {QuestionText}", question.Text);
                    _logger.LogInformation("Options: [{Options}]", string.Join(", ", question.Options));
                    _logger.LogInformation("AI provided CorrectAnswerIndex: {ProvidedIndex}", question.CorrectAnswerIndex);
                    
                    // Validate that CorrectAnswerIndex is within bounds
                    if (question.CorrectAnswerIndex < 0 || question.CorrectAnswerIndex >= question.Options.Count)
                    {
                        _logger.LogWarning("Invalid CorrectAnswerIndex {Index} for question. Options count: {OptionsCount}. Setting to 0.", 
                            question.CorrectAnswerIndex, question.Options.Count);
                        question.CorrectAnswerIndex = 0;
                    }
                    
                    _logger.LogInformation("Final CorrectAnswerIndex: {FinalIndex}", question.CorrectAnswerIndex);
                }
            }

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