using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Data.Models;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LLMQuizController : ControllerBase
    {
        private readonly ILLMQuizService _llmQuizService;
        private readonly IQuestionBankService _questionBankService;

        public LLMQuizController(ILLMQuizService llmQuizService, IQuestionBankService questionBankService)
        {
            _llmQuizService = llmQuizService;
            _questionBankService = questionBankService;
        }

        [HttpPost("generate-question")]
        public async Task<ActionResult<QuestionBank>> GenerateQuestion([FromQuery] string subject, [FromQuery] string grade, [FromQuery] string difficulty)
        {
            var question = await _llmQuizService.GenerateQuestionAsync(subject, grade, difficulty);
            await _questionBankService.CreateQuestionAsync(question);
            return Ok(question);
        }
    }
} 