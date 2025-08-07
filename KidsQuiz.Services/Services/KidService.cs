using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Kids;
using KidsQuiz.Services.Exceptions;
using KidsQuiz.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KidsQuiz.Services.Services
{
    public class KidService : IKidService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KidService> _logger;

        public KidService(ApplicationDbContext context, ILogger<KidService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<KidDto> GetKidAsync(int id)
        {
            _logger.LogInformation("Getting kid with ID: {KidId}", id);
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
            {
                _logger.LogWarning("Kid with ID {KidId} not found", id);
                throw new KidNotFoundException(id);
            }

            _logger.LogInformation("Successfully retrieved kid with ID: {KidId}", id);
            return MapToDto(kid);
        }

        public async Task<IEnumerable<KidDto>> GetAllKidsAsync()
        {
            _logger.LogInformation("Getting all kids");
            var kids = await _context.Kids.ToListAsync();
            _logger.LogInformation("Retrieved {Count} kids", kids.Count);
            return kids.Select(MapToDto);
        }

        public async Task<KidDto> CreateKidAsync(KidCreateDto kidCreateDto)
        {
            _logger.LogInformation("Creating new kid with name: {Name}, email: {Email}", kidCreateDto.Name, kidCreateDto.Email);
            
            var kid = new Kid
            {
                Name = kidCreateDto.Name,
                DateOfBirth = kidCreateDto.DateOfBirth,
                Email = kidCreateDto.Email,
                Intro = kidCreateDto.Intro,
                Grade = kidCreateDto.Grade,
                DynamicProperties = kidCreateDto.AdditionalProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString())
            };

            _context.Kids.Add(kid);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully created kid with ID: {KidId}", kid.Id);
            return MapToDto(kid);
        }

        public async Task UpdateKidAsync(int id, KidUpdateDto kidUpdateDto)
        {
            _logger.LogInformation("Updating kid with ID: {KidId}", id);
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for update", id);
                throw new KidNotFoundException(id);
            }

            kid.Name = kidUpdateDto.Name ?? kid.Name;
            kid.DateOfBirth = kidUpdateDto.DateOfBirth ?? kid.DateOfBirth;
            kid.Email = kidUpdateDto.Email ?? kid.Email;
            kid.Intro = kidUpdateDto.Intro ?? kid.Intro;

            if (kidUpdateDto.AdditionalProperties != null)
            {
                foreach (var prop in kidUpdateDto.AdditionalProperties)
                {
                    kid.DynamicProperties[prop.Key] = prop.Value.ToString();
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated kid with ID: {KidId}", id);
        }

        public async Task DeleteKidAsync(int id)
        {
            _logger.LogInformation("Deleting kid with ID: {KidId}", id);
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for deletion", id);
                throw new KidNotFoundException(id);
            }

            _context.Kids.Remove(kid);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted kid with ID: {KidId}", id);
        }

        public async Task UpdateKidIntroAsync(int id, string intro)
        {
            _logger.LogInformation("Updating intro for kid with ID: {KidId}", id);
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for intro update", id);
                throw new KidNotFoundException(id);
            }

            kid.Intro = intro;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated intro for kid with ID: {KidId}", id);
        }

        public async Task UpdateKidAdditionalPropertiesAsync(int id, Dictionary<string, object> properties)
        {
            _logger.LogInformation("Updating additional properties for kid with ID: {KidId}", id);
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
            {
                _logger.LogWarning("Kid with ID {KidId} not found for properties update", id);
                throw new KidNotFoundException(id);
            }

            foreach (var prop in properties)
            {
                kid.DynamicProperties[prop.Key] = prop.Value.ToString();
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated additional properties for kid with ID: {KidId}", id);
        }

        public async Task<KidDto> LoginKidAsync(string email, string name)
        {
            _logger.LogInformation("Attempting login for kid with email: {Email}, name: {Name}", email, name);
            var kid = await _context.Kids
                .FirstOrDefaultAsync(k => k.Email == email && k.Name == name);
            
            if (kid == null)
            {
                _logger.LogWarning("Login failed - kid not found with email: {Email}, name: {Name}", email, name);
                throw new KidNotFoundException(0); // We don't have the ID, so use 0
            }

            _logger.LogInformation("Successful login for kid with ID: {KidId}", kid.Id);
            return MapToDto(kid);
        }

        private KidDto MapToDto(Kid kid)
        {
            return new KidDto
            {
                Id = kid.Id,
                Name = kid.Name,
                DateOfBirth = kid.DateOfBirth,
                Email = kid.Email,
                Intro = kid.Intro,
                AdditionalProperties = kid.DynamicProperties.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value)
            };
        }
    }
} 