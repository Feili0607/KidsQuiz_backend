using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Kids;
using KidsQuiz.Services.Exceptions;
using KidsQuiz.Data;
using Microsoft.EntityFrameworkCore;

namespace KidsQuiz.Services.Services
{
    public class KidService : IKidService
    {
        private readonly ApplicationDbContext _context;

        public KidService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KidDto> GetKidAsync(int id)
        {
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
                throw new KidNotFoundException(id);

            return MapToDto(kid);
        }

        public async Task<IEnumerable<KidDto>> GetAllKidsAsync()
        {
            var kids = await _context.Kids.ToListAsync();
            return kids.Select(MapToDto);
        }

        public async Task<KidDto> CreateKidAsync(KidCreateDto kidCreateDto)
        {
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

            return MapToDto(kid);
        }

        public async Task UpdateKidAsync(int id, KidUpdateDto kidUpdateDto)
        {
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
                throw new KidNotFoundException(id);

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
        }

        public async Task DeleteKidAsync(int id)
        {
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
                throw new KidNotFoundException(id);

            _context.Kids.Remove(kid);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateKidIntroAsync(int id, string intro)
        {
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
                throw new KidNotFoundException(id);

            kid.Intro = intro;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateKidAdditionalPropertiesAsync(int id, Dictionary<string, object> properties)
        {
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
                throw new KidNotFoundException(id);

            foreach (var prop in properties)
            {
                kid.DynamicProperties[prop.Key] = prop.Value.ToString();
            }

            await _context.SaveChangesAsync();
        }

        public async Task<KidDto> LoginKidAsync(string email, string name)
        {
            var kid = await _context.Kids
                .FirstOrDefaultAsync(k => k.Email == email && k.Name == name);
            
            if (kid == null)
                throw new KidNotFoundException(0); // We don't have the ID, so use 0

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