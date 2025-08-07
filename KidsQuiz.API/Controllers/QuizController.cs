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