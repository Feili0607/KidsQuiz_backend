using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Quizzes;
using KidsQuiz.Services.Exceptions;
using Microsoft.Extensions.Logging;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAllQuizzes()
        {
            _logger.LogInformation("Getting all quizzes");
            var quizzes = await _quizService.GetAllQuizzesAsync();
            _logger.LogInformation("Successfully retrieved {Count} quizzes", quizzes.Count());
            return Ok(quizzes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuiz(int id)
        {
            try
            {
                _logger.LogInformation("Getting quiz with ID: {QuizId}", id);
                var quiz = await _quizService.GetQuizAsync(id);
                _logger.LogInformation("Successfully retrieved quiz with ID: {QuizId}", id);
                return Ok(quiz);
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found", id);
                return NotFound();
            }
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<QuizDetailDto>> GetQuizDetail(int id)
        {
            try
            {
                _logger.LogInformation("Getting quiz detail with ID: {QuizId}", id);
                var quiz = await _quizService.GetQuizDetailAsync(id);
                _logger.LogInformation("Successfully retrieved quiz detail with ID: {QuizId}", id);
                return Ok(quiz);
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz detail with ID {QuizId} not found", id);
                return NotFound();
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesForUser(int userId)
        {
            _logger.LogInformation("Getting quizzes for user with ID: {UserId}", userId);
            var quizzes = await _quizService.GetQuizzesByKidIdAsync(userId);
            _logger.LogInformation("Successfully retrieved {Count} quizzes for user {UserId}", quizzes.Count(), userId);
            return Ok(quizzes);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuizDto>> UpdateQuiz(int id, [FromBody] QuizUpdateDto quizUpdateDto)
        {
            try
            {
                _logger.LogInformation("Received quiz update request for ID: {QuizId}", id);
                _logger.LogInformation("Update data: Title='{Title}', Description='{Description}', Content='{Content}', DifficultyLevel={DifficultyLevel}, Labels={Labels}", 
                    quizUpdateDto?.Title, quizUpdateDto?.Description, quizUpdateDto?.Content, quizUpdateDto?.DifficultyLevel, 
                    quizUpdateDto?.Labels != null ? string.Join(",", quizUpdateDto.Labels) : "null");
                
                var updatedQuiz = await _quizService.UpdateQuizAsync(id, quizUpdateDto);
                _logger.LogInformation("Successfully updated quiz with ID: {QuizId}", id);
                
                // Log the returned quiz data for debugging
                _logger.LogInformation("Returned quiz data: Title='{Title}', DifficultyLevel={DifficultyLevel}", 
                    updatedQuiz.Title, updatedQuiz.DifficultyLevel);
                
                return Ok(updatedQuiz);
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found for update", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz with ID {QuizId}: {Message}", id, ex.Message);
                return BadRequest($"Failed to update quiz: {ex.Message}");
            }
        }

        [HttpPost("{id}/fix-answer-indices")]
        public async Task<ActionResult> FixQuizAnswerIndices(int id)
        {
            try
            {
                _logger.LogInformation("Fixing answer indices for quiz with ID: {QuizId}", id);
                var hasChanges = await _quizService.FixQuizAnswerIndicesAsync(id);
                
                if (hasChanges)
                {
                    _logger.LogInformation("Successfully fixed answer indices for quiz {QuizId}", id);
                    return Ok(new { message = "Quiz answer indices have been fixed successfully.", quizId = id });
                }
                else
                {
                    _logger.LogInformation("No answer index fixes needed for quiz {QuizId}", id);
                    return Ok(new { message = "No answer index fixes were needed for this quiz.", quizId = id });
                }
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found for answer index fix", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing answer indices for quiz with ID {QuizId}: {Message}", id, ex.Message);
                return BadRequest($"Failed to fix quiz answer indices: {ex.Message}");
            }
        }

        [HttpPost("generate-openai")]
        public async Task<ActionResult<QuizDetailDto>> GenerateOpenAIQuiz([FromBody] GenerateOpenAIQuizRequest request)
        {
            try
            {
                _logger.LogInformation("Received quiz generation request for user {UserId}", request.UserId);
                _logger.LogInformation("User info: Name={Name}, Grade={Grade}, Subject={Subject}", 
                    request.UserInfo?.Name, request.UserInfo?.Grade, request.UserInfo?.Subject);
                
                if (request.UserInfo == null)
                {
                    _logger.LogWarning("UserInfo is null for user {UserId}", request.UserId);
                    return BadRequest("UserInfo is required");
                }
                
                if (string.IsNullOrEmpty(request.UserInfo.Grade))
                {
                    _logger.LogWarning("Grade is missing for user {UserId}", request.UserId);
                    return BadRequest("Grade is required");
                }

                var quiz = await _quizService.GenerateQuizUsingLLMAsync(request.UserInfo, request.UserId);
                _logger.LogInformation("Successfully generated quiz with ID: {QuizId} for user {UserId}", quiz.Id, request.UserId);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GenerateOpenAIQuiz for user {UserId}: {Message}", request.UserId, ex.Message);
                return BadRequest($"Failed to generate quiz: {ex.Message}");
            }
        }
    }

    public class GenerateOpenAIQuizRequest
    {
        public int UserId { get; set; }
        public UserInfoDto UserInfo { get; set; }
    }
} 