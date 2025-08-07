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
using Microsoft.Extensions.Logging;

namespace KidsQuiz.Services.Services
{
    public class QuizSolvingRecordService : IQuizSolvingRecordService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuizSolvingRecordService> _logger;

        public QuizSolvingRecordService(ApplicationDbContext context, ILogger<QuizSolvingRecordService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<QuizRecordDto>> GetAllRecordsAsync()
        {
            _logger.LogInformation("Getting all quiz solving records");
            var records = await _context.QuizSolvingRecords.ToListAsync();
            _logger.LogInformation("Retrieved {Count} quiz solving records", records.Count);
            return records.Select(MapToDto);
        }

        public async Task<QuizRecordDto> GetRecordAsync(int id)
        {
            _logger.LogInformation("Getting quiz solving record with ID: {RecordId}", id);
            var record = await _context.QuizSolvingRecords.FindAsync(id);
            if (record == null)
            {
                _logger.LogWarning("Quiz solving record with ID {RecordId} not found", id);
                throw new QuizRecordNotFoundException(id);
            }
            
            _logger.LogInformation("Successfully retrieved quiz solving record with ID: {RecordId}", id);
            return MapToDto(record);
        }

        public async Task<IEnumerable<QuizRecordDto>> GetRecordsByKidAsync(int kidId)
        {
            _logger.LogInformation("Getting quiz solving records for kid with ID: {KidId}", kidId);
            var records = await _context.QuizSolvingRecords
                .Where(r => r.KidId == kidId)
                .ToListAsync();
            _logger.LogInformation("Retrieved {Count} quiz solving records for kid with ID: {KidId}", records.Count, kidId);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<QuizRecordDto>> GetRecordsByQuizAsync(int quizId)
        {
            _logger.LogInformation("Getting quiz solving records for quiz with ID: {QuizId}", quizId);
            var records = await _context.QuizSolvingRecords
                .Where(r => r.QuizId == quizId)
                .ToListAsync();
            _logger.LogInformation("Retrieved {Count} quiz solving records for quiz with ID: {QuizId}", records.Count, quizId);
            return records.Select(MapToDto);
        }

        public async Task<QuizRecordDto> CreateRecordAsync(QuizRecordDto recordDto)
        {
            _logger.LogInformation("Creating new quiz solving record for kid {KidId}, quiz {QuizId}", recordDto.KidId, recordDto.QuizId);
            
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
            
            _logger.LogInformation("Successfully created quiz solving record with ID: {RecordId}", record.Id);
            return MapToDto(record);
        }

        public async Task<QuizResultDto> SubmitQuizAnswersAsync(QuizRecordDto recordDto)
        {
            _logger.LogInformation("Submitting quiz answers for record with ID: {RecordId}", recordDto.Id);
            var record = await _context.QuizSolvingRecords.FindAsync(recordDto.Id);
            if (record == null)
            {
                _logger.LogWarning("Quiz solving record with ID {RecordId} not found for answer submission", recordDto.Id);
                throw new QuizRecordNotFoundException(recordDto.Id);
            }

            record.CompletedAt = DateTime.UtcNow;
            record.Answers = recordDto.Answers;
            record.Score = recordDto.Score;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully submitted quiz answers for record {RecordId} with score {Score}", record.Id, record.Score);

            return new QuizResultDto
            {
                RecordId = record.Id,
                Score = record.Score,
                TimeTakenInSeconds = record.TimeTakenInSeconds
            };
        }

        public async Task<Dictionary<string, object>> GetKidQuizStatsAsync(int kidId)
        {
            _logger.LogInformation("Getting quiz stats for kid with ID: {KidId}", kidId);
            var records = await _context.QuizSolvingRecords
                .Where(r => r.KidId == kidId)
                .ToListAsync();

            var stats = new Dictionary<string, object>
            {
                { "TotalQuizzes", records.Count },
                { "AverageScore", records.Average(r => r.Score) },
                { "TotalTimeSpent", records.Sum(r => r.TimeTakenInSeconds) }
            };

            _logger.LogInformation("Retrieved quiz stats for kid {KidId}: TotalQuizzes={TotalQuizzes}, AverageScore={AverageScore}, TotalTimeSpent={TotalTimeSpent}", 
                kidId, stats["TotalQuizzes"], stats["AverageScore"], stats["TotalTimeSpent"]);

            return stats;
        }

        public async Task<Dictionary<string, object>> GetKidProgressAsync(int kidId)
        {
            _logger.LogInformation("Getting progress for kid with ID: {KidId}", kidId);
            var records = await _context.QuizSolvingRecords
                .Where(r => r.KidId == kidId)
                .OrderBy(r => r.StartedAt)
                .ToListAsync();

            var progress = new Dictionary<string, object>
            {
                { "QuizHistory", records.Select(r => new
                    {
                        QuizId = r.QuizId,
                        Score = r.Score,
                        CompletedAt = r.CompletedAt
                    }).ToList()
                }
            };

            _logger.LogInformation("Retrieved progress for kid {KidId} with {HistoryCount} quiz history entries", kidId, records.Count);

            return progress;
        }

        public async Task<IEnumerable<QuizSummaryDto>> GetKidRecommendationsAsync(int kidId)
        {
            _logger.LogInformation("Getting recommendations for kid with ID: {KidId}", kidId);
            // TODO: Implement recommendation logic
            _logger.LogInformation("Recommendations not yet implemented for kid {KidId}", kidId);
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