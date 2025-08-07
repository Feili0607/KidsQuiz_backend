using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Quizzes;
using KidsQuiz.Services.Exceptions;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetAllQuizzes()
        {
            var quizzes = await _quizService.GetAllQuizzesAsync();
            return Ok(quizzes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuiz(int id)
        {
            try
            {
                var quiz = await _quizService.GetQuizAsync(id);
                return Ok(quiz);
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult<QuizDetailDto>> GetQuizDetail(int id)
        {
            try
            {
                var quiz = await _quizService.GetQuizDetailAsync(id);
                return Ok(quiz);
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesForUser(int userId)
        {
            var quizzes = await _quizService.GetQuizzesByKidIdAsync(userId);
            return Ok(quizzes);
        }

        [HttpPost("generate-openai")]
        public async Task<ActionResult<QuizDetailDto>> GenerateOpenAIQuiz([FromBody] GenerateOpenAIQuizRequest request)
        {
            try
            {
                Console.WriteLine($"Received quiz generation request for user {request.UserId}");
                Console.WriteLine($"User info: Name={request.UserInfo?.Name}, Grade={request.UserInfo?.Grade}, Subject={request.UserInfo?.Subject}");
                
                if (request.UserInfo == null)
                {
                    Console.WriteLine("UserInfo is null!");
                    return BadRequest("UserInfo is required");
                }
                
                if (string.IsNullOrEmpty(request.UserInfo.Grade))
                {
                    Console.WriteLine("Grade is missing!");
                    return BadRequest("Grade is required");
                }

                var quiz = await _quizService.GenerateQuizUsingLLMAsync(request.UserInfo, request.UserId);
                Console.WriteLine($"Successfully generated quiz with ID: {quiz.Id}");
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GenerateOpenAIQuiz: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
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