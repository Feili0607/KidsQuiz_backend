using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Kids;
using KidsQuiz.Services.Exceptions;
using Microsoft.Extensions.Logging;

namespace KidsQuiz.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KidsController : ControllerBase
    {
        private readonly IKidService _kidsService;
        private readonly ILogger<KidsController> _logger;

        public KidsController(IKidService kidsService, ILogger<KidsController> logger)
        {
            _kidsService = kidsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KidDto>>> GetAllKids()
        {
            _logger.LogInformation("Getting all kids");
            var kids = await _kidsService.GetAllKidsAsync();
            _logger.LogInformation("Successfully retrieved {Count} kids", kids.Count());
            return Ok(kids);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KidDto>> GetKid(int id)
        {
            try
            {
                _logger.LogInformation("Getting kid with ID: {KidId}", id);
                var kid = await _kidsService.GetKidAsync(id);
                _logger.LogInformation("Successfully retrieved kid with ID: {KidId}", id);
                return Ok(kid);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found", id);
                return NotFound();
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<KidDto>> RegisterKid([FromBody] KidCreateDto kidCreateDto)
        {
            try
            {
                _logger.LogInformation("Registering new kid with name: {Name}, email: {Email}", kidCreateDto.Name, kidCreateDto.Email);
                
                // Validate required fields
                if (string.IsNullOrWhiteSpace(kidCreateDto.Name))
                {
                    _logger.LogWarning("Registration failed - Name is required");
                    return BadRequest("Name is required");
                }
                if (string.IsNullOrWhiteSpace(kidCreateDto.Email))
                {
                    _logger.LogWarning("Registration failed - Email is required");
                    return BadRequest("Email is required");
                }
                if (string.IsNullOrWhiteSpace(kidCreateDto.Grade))
                {
                    _logger.LogWarning("Registration failed - Grade is required");
                    return BadRequest("Grade is required");
                }
                if (kidCreateDto.DateOfBirth == default)
                {
                    _logger.LogWarning("Registration failed - Date of birth is required");
                    return BadRequest("Date of birth is required");
                }

                var kid = await _kidsService.CreateKidAsync(kidCreateDto);
                _logger.LogInformation("Successfully registered kid with ID: {KidId}", kid.Id);
                return CreatedAtAction(nameof(GetKid), new { id = kid.Id }, kid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering kid: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<KidDto>> LoginKid([FromBody] KidLoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for kid with email: {Email}, name: {Name}", loginDto.Email, loginDto.Name);
                
                if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Name))
                {
                    _logger.LogWarning("Login failed - Email and name are required");
                    return BadRequest("Email and name are required");
                }

                var kid = await _kidsService.LoginKidAsync(loginDto.Email, loginDto.Name);
                _logger.LogInformation("Successful login for kid with ID: {KidId}", kid.Id);
                return Ok(kid);
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Login failed - Kid not found with email: {Email}, name: {Name}", loginDto.Email, loginDto.Name);
                return NotFound("Kid not found with the provided email and name");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<KidDto>> CreateKid(KidCreateDto kidCreateDto)
        {
            _logger.LogInformation("Creating new kid with name: {Name}, email: {Email}", kidCreateDto.Name, kidCreateDto.Email);
            var kid = await _kidsService.CreateKidAsync(kidCreateDto);
            _logger.LogInformation("Successfully created kid with ID: {KidId}", kid.Id);
            return CreatedAtAction(nameof(GetKid), new { id = kid.Id }, kid);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKid(int id, KidUpdateDto kidUpdateDto)
        {
            try
            {
                _logger.LogInformation("Updating kid with ID: {KidId}", id);
                await _kidsService.UpdateKidAsync(id, kidUpdateDto);
                _logger.LogInformation("Successfully updated kid with ID: {KidId}", id);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for update", id);
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKid(int id)
        {
            try
            {
                _logger.LogInformation("Deleting kid with ID: {KidId}", id);
                await _kidsService.DeleteKidAsync(id);
                _logger.LogInformation("Successfully deleted kid with ID: {KidId}", id);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for deletion", id);
                return NotFound();
            }
        }

        [HttpPut("{id}/intro")]
        public async Task<IActionResult> UpdateKidIntro(int id, [FromBody] string intro)
        {
            try
            {
                _logger.LogInformation("Updating intro for kid with ID: {KidId}", id);
                await _kidsService.UpdateKidIntroAsync(id, intro);
                _logger.LogInformation("Successfully updated intro for kid with ID: {KidId}", id);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for intro update", id);
                return NotFound();
            }
        }

        [HttpPut("{id}/properties")]
        public async Task<IActionResult> UpdateKidProperties(int id, [FromBody] Dictionary<string, object> properties)
        {
            try
            {
                _logger.LogInformation("Updating properties for kid with ID: {KidId}", id);
                await _kidsService.UpdateKidAdditionalPropertiesAsync(id, properties);
                _logger.LogInformation("Successfully updated properties for kid with ID: {KidId}", id);
                return NoContent();
            }
            catch (KidNotFoundException)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for properties update", id);
                return NotFound();
            }
        }
    }
} 