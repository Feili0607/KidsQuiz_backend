using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Records;
using KidsQuiz.Services.Exceptions;


namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizSolvingRecordsController : ControllerBase
    {
        private readonly IQuizSolvingRecordService _recordService;

        public QuizSolvingRecordsController(IQuizSolvingRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuizRecordDto>>> GetAllRecords()
        {
            var records = await _recordService.GetAllRecordsAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizRecordDto>> GetRecord(int id)
        {
            try
            {
                var record = await _recordService.GetRecordAsync(id);
                return Ok(record);
            }
            catch (QuizRecordNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("kid/{kidId}")]
        public async Task<ActionResult<IEnumerable<QuizRecordDto>>> GetRecordsByKid(int kidId)
        {
            try
            {
                var records = await _recordService.GetRecordsByKidAsync(kidId);
                return Ok(records);
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<IEnumerable<QuizRecordDto>>> GetRecordsByQuiz(int quizId)
        {
            try
            {
                var records = await _recordService.GetRecordsByQuizAsync(quizId);
                return Ok(records);
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<QuizRecordDto>> CreateRecord(QuizRecordDto recordDto)
        {
            try
            {
                var record = await _recordService.CreateRecordAsync(recordDto);
                return CreatedAtAction(nameof(GetRecord), new { id = record.Id }, record);
            }
            catch (QuizRecordValidationException ex)
            {
                return BadRequest(ex.ValidationErrors);
            }
            catch (KidNotFoundException)
            {
                return NotFound("Kid not found");
            }
            catch (QuizNotFoundException)
            {
                return NotFound("Quiz not found");
            }
        }

        [HttpPost("submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuizAnswers([FromBody] QuizRecordDto recordDto)
        {
            try
            {
                var result = await _recordService.SubmitQuizAnswersAsync(recordDto);
                return Ok(result);
            }
            catch (QuizRecordValidationException ex)
            {
                return BadRequest(ex.ValidationErrors);
            }
            catch (KidNotFoundException)
            {
                return NotFound("Kid not found");
            }
            catch (QuizNotFoundException)
            {
                return NotFound("Quiz not found");
            }
        }

        [HttpGet("kid/{kidId}/stats")]
        public async Task<ActionResult<Dictionary<string, object>>> GetKidQuizStats(int kidId)
        {
            try
            {
                var stats = await _recordService.GetKidQuizStatsAsync(kidId);
                return Ok(stats);
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("kid/{kidId}/progress")]
        public async Task<ActionResult<Dictionary<string, object>>> GetKidProgress(int kidId)
        {
            try
            {
                var progress = await _recordService.GetKidProgressAsync(kidId);
                return Ok(progress);
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("kid/{kidId}/recommendations")]
        public async Task<ActionResult<IEnumerable<QuizSummaryDto>>> GetKidRecommendations(int kidId)
        {
            try
            {
                var recommendations = await _recordService.GetKidRecommendationsAsync(kidId);
                return Ok(recommendations);
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }
    }
} 