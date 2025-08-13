using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using KidsQuiz.Data;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.DTOs.Records;
using KidsQuiz.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
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

            // Get the quiz with questions to validate answers
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == record.QuizId);

            if (quiz == null)
            {
                _logger.LogWarning("Quiz with ID {QuizId} not found for answer validation", record.QuizId);
                throw new QuizNotFoundException(record.QuizId);
            }

            // Parse the answers JSON
            var userAnswers = JsonSerializer.Deserialize<List<UserAnswerDto>>(recordDto.Answers ?? "[]", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (userAnswers == null)
            {
                _logger.LogWarning("No answers provided for quiz record {RecordId}", record.Id);
                userAnswers = new List<UserAnswerDto>();
            }

            // Validate answers and calculate score
            var validationResult = await ValidateAnswersAsync(quiz.Questions.ToList(), userAnswers);
            
            // Update the record with validated results
            record.CompletedAt = DateTime.UtcNow;
            record.Answers = recordDto.Answers;
            record.Score = validationResult.CorrectAnswers;
            record.TimeTakenInSeconds = recordDto.TimeTakenInSeconds;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully submitted quiz answers for record {RecordId}. Score: {Score}/{TotalQuestions}", 
                record.Id, validationResult.CorrectAnswers, validationResult.TotalQuestions);

            return new QuizResultDto
            {
                RecordId = record.Id,
                Score = validationResult.CorrectAnswers,
                TotalQuestions = validationResult.TotalQuestions,
                CorrectAnswers = validationResult.CorrectAnswers,
                AccuracyPercentage = validationResult.AccuracyPercentage,
                TimeTakenInSeconds = record.TimeTakenInSeconds,
                DetailedResults = validationResult.DetailedResults
            };
        }

        private async Task<AnswerValidationResult> ValidateAnswersAsync(List<Question> questions, List<UserAnswerDto> userAnswers)
        {
            var totalQuestions = questions.Count;
            var correctAnswers = 0;
            var detailedResults = new Dictionary<string, object>();
            var questionResults = new List<QuestionResultDto>();

            _logger.LogInformation("Validating {UserAnswerCount} user answers against {QuestionCount} questions", 
                userAnswers.Count, totalQuestions);

            foreach (var question in questions)
            {
                var userAnswer = userAnswers.FirstOrDefault(ua => ua.QuestionId == question.Id);
                var isCorrect = false;
                var selectedAnswerIndex = -1;

                if (userAnswer != null)
                {
                    selectedAnswerIndex = userAnswer.SelectedAnswerIndex;
                    isCorrect = selectedAnswerIndex == question.CorrectAnswerIndex;
                    
                    if (isCorrect)
                    {
                        correctAnswers++;
                        _logger.LogInformation("✅ Question {QuestionId}: CORRECT! User selected {SelectedIndex}, Correct was {CorrectIndex}", 
                            question.Id, selectedAnswerIndex, question.CorrectAnswerIndex);
                    }
                    else
                    {
                        _logger.LogInformation("❌ Question {QuestionId}: INCORRECT! User selected {SelectedIndex}, Correct was {CorrectIndex}", 
                            question.Id, selectedAnswerIndex, question.CorrectAnswerIndex);
                    }
                }
                else
                {
                    _logger.LogInformation("⏭️ Question {QuestionId}: No answer provided by user", question.Id);
                }

                var questionResult = new QuestionResultDto
                {
                    QuestionId = question.Id,
                    QuestionText = question.Text,
                    SelectedAnswerIndex = selectedAnswerIndex,
                    CorrectAnswerIndex = question.CorrectAnswerIndex,
                    IsCorrect = isCorrect,
                    TimeTaken = TimeSpan.Zero, // TODO: Add actual time tracking
                    PointsEarned = isCorrect ? question.Points : 0,
                    Explanation = question.Explanation
                };

                questionResults.Add(questionResult);
            }

            var accuracyPercentage = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

            _logger.LogInformation("🎯 Quiz validation complete: {CorrectAnswers}/{TotalQuestions} correct ({AccuracyPercentage:F1}%)", 
                correctAnswers, totalQuestions, accuracyPercentage);

            detailedResults["QuestionResults"] = questionResults;
            detailedResults["Summary"] = new
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                IncorrectAnswers = totalQuestions - correctAnswers,
                AccuracyPercentage = accuracyPercentage
            };

            return new AnswerValidationResult
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                AccuracyPercentage = accuracyPercentage,
                DetailedResults = detailedResults
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

    // Supporting DTOs for answer validation
    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerIndex { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }

    public class AnswerValidationResult
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double AccuracyPercentage { get; set; }
        public Dictionary<string, object> DetailedResults { get; set; }
    }
} 