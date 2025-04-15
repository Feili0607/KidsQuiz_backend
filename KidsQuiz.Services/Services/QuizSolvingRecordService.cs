using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KidsQuiz.Data;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Records;
using KidsQuiz.Services.Exceptions;

namespace KidsQuiz.Services.Services
{
    public class QuizSolvingRecordService : IQuizSolvingRecordService
    {
        private readonly ApplicationDbContext _context;

        public QuizSolvingRecordService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuizRecordDto>> GetAllRecordsAsync()
        {
            var records = await _context.QuizSolvingRecords.ToListAsync();
            return records.Select(MapToDto);
        }

        public async Task<QuizRecordDto> GetRecordAsync(int id)
        {
            var record = await _context.QuizSolvingRecords.FindAsync(id);
            if (record == null) throw new QuizRecordNotFoundException(id);
            return MapToDto(record);
        }

        public async Task<IEnumerable<QuizRecordDto>> GetRecordsByKidAsync(int kidId)
        {
            var records = await _context.QuizSolvingRecords
                .Where(r => r.KidId == kidId)
                .ToListAsync();
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<QuizRecordDto>> GetRecordsByQuizAsync(int quizId)
        {
            var records = await _context.QuizSolvingRecords
                .Where(r => r.QuizId == quizId)
                .ToListAsync();
            return records.Select(MapToDto);
        }

        public async Task<QuizRecordDto> CreateRecordAsync(QuizRecordDto recordDto)
        {
            var record = new QuizSolvingRecord
            {
                KidId = recordDto.KidId,
                QuizId = recordDto.QuizId,
                StartedAt = DateTime.UtcNow,
                Answers = recordDto.Answers,
                TimeTakenInSeconds = recordDto.TimeTakenInSeconds
            };

            _context.QuizSolvingRecords.Add(record);
            await _context.SaveChangesAsync();
            return MapToDto(record);
        }

        public async Task<QuizResultDto> SubmitQuizAnswersAsync(QuizRecordDto recordDto)
        {
            var record = await _context.QuizSolvingRecords.FindAsync(recordDto.Id);
            if (record == null) throw new QuizRecordNotFoundException(recordDto.Id);

            record.CompletedAt = DateTime.UtcNow;
            record.Answers = recordDto.Answers;
            record.Score = recordDto.Score;
            await _context.SaveChangesAsync();

            return new QuizResultDto
            {
                RecordId = record.Id,
                Score = record.Score,
                TimeTakenInSeconds = record.TimeTakenInSeconds
            };
        }

        public async Task<Dictionary<string, object>> GetKidQuizStatsAsync(int kidId)
        {
            var records = await _context.QuizSolvingRecords
                .Where(r => r.KidId == kidId)
                .ToListAsync();

            return new Dictionary<string, object>
            {
                { "TotalQuizzes", records.Count },
                { "AverageScore", records.Average(r => r.Score) },
                { "TotalTimeSpent", records.Sum(r => r.TimeTakenInSeconds) }
            };
        }

        public async Task<Dictionary<string, object>> GetKidProgressAsync(int kidId)
        {
            var records = await _context.QuizSolvingRecords
                .Where(r => r.KidId == kidId)
                .OrderBy(r => r.StartedAt)
                .ToListAsync();

            return new Dictionary<string, object>
            {
                { "QuizHistory", records.Select(r => new
                    {
                        QuizId = r.QuizId,
                        Score = r.Score,
                        CompletedAt = r.CompletedAt
                    }).ToList()
                }
            };
        }

        public async Task<IEnumerable<QuizSummaryDto>> GetKidRecommendationsAsync(int kidId)
        {
            // TODO: Implement recommendation logic
            return new List<QuizSummaryDto>();
        }

        private QuizRecordDto MapToDto(QuizSolvingRecord record)
        {
            return new QuizRecordDto
            {
                Id = record.Id,
                KidId = record.KidId,
                QuizId = record.QuizId,
                StartedAt = record.StartedAt,
                CompletedAt = record.CompletedAt,
                Score = record.Score,
                Answers = record.Answers,
                TimeTakenInSeconds = record.TimeTakenInSeconds
            };
        }
    }
} 