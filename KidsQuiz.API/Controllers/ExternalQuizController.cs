using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Data.Models;
using System.Collections.Generic;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalQuizController : ControllerBase
    {
        private readonly IExternalQuizService _externalQuizService;
        private readonly IQuestionBankService _questionBankService;

        public ExternalQuizController(
            IExternalQuizService externalQuizService,
            IQuestionBankService questionBankService)
        {
            _externalQuizService = externalQuizService;
            _questionBankService = questionBankService;
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<IEnumerable<QuestionBank>>> FetchQuestions(
            [FromQuery] int count = 10,
            [FromQuery] string category = null,
            [FromQuery] string difficulty = null)
        {
            var questions = await _externalQuizService.FetchQuestionsFromOpenTDBAsync(
                count, category, difficulty);

            // Save questions to our question bank
            foreach (var question in questions)
            {
                await _questionBankService.CreateQuestionAsync(question);
            }

            return Ok(questions);
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<string>> GetCategories()
        {
            // OpenTDB categories
            var categories = new[]
            {
                "General Knowledge",
                "Science: Computers",
                "Science: Mathematics",
                "Science: Nature",
                "Science: Gadgets",
                "Entertainment: Books",
                "Entertainment: Film",
                "Entertainment: Music",
                "Entertainment: Video Games",
                "Sports",
                "Geography",
                "History",
                "Politics",
                "Art",
                "Celebrities",
                "Animals"
            };

            return Ok(categories);
        }
    }
} 