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
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<ActionResult<QuizDto>> CreateQuiz(QuizCreateDto quizCreateDto)
        {
            var quiz = await _quizService.CreateQuizAsync(quizCreateDto);
            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, QuizUpdateDto quizUpdateDto)
        {
            try
            {
                await _quizService.UpdateQuizAsync(id, quizUpdateDto);
                return NoContent();
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            try
            {
                await _quizService.DeleteQuizAsync(id);
                return NoContent();
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/rating")]
        public async Task<IActionResult> UpdateQuizRating(int id, [FromBody] double rating)
        {
            try
            {
                await _quizService.UpdateQuizRatingAsync(id, rating);
                return NoContent();
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/labels")]
        public async Task<IActionResult> AddLabel(int id, [FromBody] string label)
        {
            try
            {
                await _quizService.AddLabelToQuizAsync(id, label);
                return NoContent();
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}/labels/{label}")]
        public async Task<IActionResult> RemoveLabel(int id, string label)
        {
            try
            {
                await _quizService.RemoveLabelFromQuizAsync(id, label);
                return NoContent();
            }
            catch (QuizNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("by-labels")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesByLabels([FromQuery] List<string> labels)
        {
            var quizzes = await _quizService.GetQuizzesByLabelsAsync(labels);
            return Ok(quizzes);
        }

        [HttpGet("by-difficulty/{difficultyLevel}")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzesByDifficulty(int difficultyLevel)
        {
            var quizzes = await _quizService.GetQuizzesByDifficultyAsync(difficultyLevel);
            return Ok(quizzes);
        }
    }
} 