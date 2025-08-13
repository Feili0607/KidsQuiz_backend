using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Records;
using KidsQuiz.Services.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizSolvingRecordsController : ControllerBase
    {
        private readonly IQuizSolvingRecordService _recordService;
        private readonly IQuizService _quizService; // Added _quizService
        private readonly ILogger<QuizSolvingRecordsController> _logger;

        public QuizSolvingRecordsController(IQuizSolvingRecordService recordService, IQuizService quizService, ILogger<QuizSolvingRecordsController> logger) // Modified constructor
        {
            _recordService = recordService;
            _quizService = quizService; // Initialize _quizService
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizRecordDto>>> GetAllRecords()
        {
            _logger.LogInformation("Getting all quiz solving records");
            var records = await _recordService.GetAllRecordsAsync();
            _logger.LogInformation("Successfully retrieved {Count} quiz solving records", records.Count());
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizRecordDto>> GetRecord(int id)
        {
            try
            {
                _logger.LogInformation("Getting quiz solving record with ID: {RecordId}", id);
                var record = await _recordService.GetRecordAsync(id);
                _logger.LogInformation("Successfully retrieved quiz solving record with ID: {RecordId}", id);
                return Ok(record);
            }
            catch (QuizRecordNotFoundException)
            {
                _logger.LogWarning("Quiz solving record with ID {RecordId} not found", id);
                return NotFound();
            }
        }

        [HttpGet("kid/{kidId}")]
        public async Task<ActionResult<IEnumerable<QuizRecordDto>>> GetRecordsByKid(int kidId)
        {
            try
            {
                _logger.LogInformation("Getting quiz solving records for kid with ID: {KidId}", kidId);
                var records = await _recordService.GetRecordsByKidAsync(kidId);
                _logger.LogInformation("Successfully retrieved {Count} quiz solving records for kid {KidId}", records.Count(), kidId);
                return Ok(records);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found", kidId);
                return NotFound();
            }
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<QuizRecordDto>>> GetRecordsByQuiz(int quizId)
        {
            try
            {
                _logger.LogInformation("Getting quiz solving records for quiz with ID: {QuizId}", quizId);
                var records = await _recordService.GetRecordsByQuizAsync(quizId);
                _logger.LogInformation("Successfully retrieved {Count} quiz solving records for quiz {QuizId}", records.Count(), quizId);
                return Ok(records);
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found", quizId);
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<QuizRecordDto>> CreateRecord(QuizRecordDto recordDto)
        {
            try
            {
                _logger.LogInformation("Creating new quiz solving record for kid {KidId}, quiz {QuizId}", recordDto.KidId, recordDto.QuizId);
                var record = await _recordService.CreateRecordAsync(recordDto);
                _logger.LogInformation("Successfully created quiz solving record with ID: {RecordId}", record.Id);
                return CreatedAtAction(nameof(GetRecord), new { id = record.Id }, record);
            }
            catch (QuizRecordValidationException ex)
            {
                _logger.LogWarning("Validation error creating quiz solving record: {ValidationErrors}", ex.ValidationErrors);
                return BadRequest(ex.ValidationErrors);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid not found when creating quiz solving record");
                return NotFound("Kid not found");
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz not found when creating quiz solving record");
                return NotFound("Quiz not found");
            }
        }

        [HttpPost("submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuizAnswers([FromBody] QuizRecordDto recordDto)
        {
            try
            {
                _logger.LogInformation("Submitting quiz answers for record with ID: {RecordId}", recordDto.Id);
                var result = await _recordService.SubmitQuizAnswersAsync(recordDto);
                _logger.LogInformation("Successfully submitted quiz answers for record {RecordId} with score {Score}", recordDto.Id, result.Score);
                return Ok(result);
            }
            catch (QuizRecordValidationException ex)
            {
                _logger.LogWarning("Validation error submitting quiz answers: {ValidationErrors}", ex.ValidationErrors);
                return BadRequest(ex.ValidationErrors);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid not found when submitting quiz answers");
                return NotFound("Kid not found");
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz not found when submitting quiz answers");
                return NotFound("Quiz not found");
            }
        }

        [HttpGet("results/{recordId}")]
        public async Task<ActionResult<QuizResultDto>> GetQuizResults(int recordId)
        {
            try
            {
                _logger.LogInformation("Getting quiz results for record with ID: {RecordId}", recordId);
                var record = await _recordService.GetRecordAsync(recordId);
                
                // Get the quiz with questions to show correct answers
                var quiz = await _quizService.GetQuizDetailAsync(record.QuizId);
                
                // Parse the user answers
                var userAnswers = JsonSerializer.Deserialize<List<UserAnswerDto>>(record.Answers ?? "[]", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var result = new QuizResultDto
                {
                    RecordId = record.Id,
                    Score = record.Score,
                    TotalQuestions = quiz.Questions.Count,
                    CorrectAnswers = record.Score,
                    AccuracyPercentage = quiz.Questions.Count > 0 ? (double)record.Score / quiz.Questions.Count * 100 : 0,
                    TimeTakenInSeconds = record.TimeTakenInSeconds,
                    DetailedResults = new Dictionary<string, object>
                    {
                        ["Quiz"] = quiz,
                        ["UserAnswers"] = userAnswers ?? new List<UserAnswerDto>()
                    }
                };
                
                _logger.LogInformation("Successfully retrieved quiz results for record {RecordId}", recordId);
                return Ok(result);
            }
            catch (QuizRecordNotFoundException)
            {
                _logger.LogWarning("Quiz record with ID {RecordId} not found", recordId);
                return NotFound("Quiz record not found");
            }
            catch (QuizNotFoundException)
            {
                _logger.LogWarning("Quiz not found for record {RecordId}", recordId);
                return NotFound("Quiz not found");
            }
        }

        [HttpGet("kid/{kidId}/stats")]
        public async Task<ActionResult<Dictionary<string, object>>> GetKidQuizStats(int kidId)
        {
            try
            {
                _logger.LogInformation("Getting quiz stats for kid with ID: {KidId}", kidId);
                var stats = await _recordService.GetKidQuizStatsAsync(kidId);
                _logger.LogInformation("Successfully retrieved quiz stats for kid {KidId}", kidId);
                return Ok(stats);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found when getting stats", kidId);
                return NotFound();
            }
        }

        [HttpGet("kid/{kidId}/progress")]
        public async Task<ActionResult<Dictionary<string, object>>> GetKidProgress(int kidId)
        {
            try
            {
                _logger.LogInformation("Getting progress for kid with ID: {KidId}", kidId);
                var progress = await _recordService.GetKidProgressAsync(kidId);
                _logger.LogInformation("Successfully retrieved progress for kid {KidId}", kidId);
                return Ok(progress);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found when getting progress", kidId);
                return NotFound();
            }
        }

        [HttpGet("kid/{kidId}/recommendations")]
        public async Task<ActionResult<IEnumerable<QuizSummaryDto>>> GetKidRecommendations(int kidId)
        {
            try
            {
                _logger.LogInformation("Getting recommendations for kid with ID: {KidId}", kidId);
                var recommendations = await _recordService.GetKidRecommendationsAsync(kidId);
                _logger.LogInformation("Successfully retrieved recommendations for kid {KidId}", kidId);
                return Ok(recommendations);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found when getting recommendations", kidId);
                return NotFound();
            }
        }
    }

    // Supporting DTO for answer validation
    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }
} 