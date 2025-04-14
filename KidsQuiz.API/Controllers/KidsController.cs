using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Kids;
using KidsQuiz.Services.Exceptions;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KidsController : ControllerBase
    {
        private readonly IKidService _kidsService;

        public KidsController(IKidService kidsService)
        {
            _kidsService = kidsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KidDto>>> GetAllKids()
        {
            var kids = await _kidsService.GetAllKidsAsync();
            return Ok(kids);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KidDto>> GetKid(int id)
        {
            try
            {
                var kid = await _kidsService.GetKidAsync(id);
                return Ok(kid);
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<KidDto>> CreateKid(KidCreateDto kidCreateDto)
        {
            var kid = await _kidsService.CreateKidAsync(kidCreateDto);
            return CreatedAtAction(nameof(GetKid), new { id = kid.Id }, kid);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKid(int id, KidUpdateDto kidUpdateDto)
        {
            try
            {
                await _kidsService.UpdateKidAsync(id, kidUpdateDto);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKid(int id)
        {
            try
            {
                await _kidsService.DeleteKidAsync(id);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/intro")]
        public async Task<IActionResult> UpdateKidIntro(int id, [FromBody] string intro)
        {
            try
            {
                await _kidsService.UpdateKidIntroAsync(id, intro);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/properties")]
        public async Task<IActionResult> UpdateKidProperties(int id, [FromBody] Dictionary<string, object> properties)
        {
            try
            {
                await _kidsService.UpdateKidAdditionalPropertiesAsync(id, properties);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                return NotFound();
            }
        }
    }
} 