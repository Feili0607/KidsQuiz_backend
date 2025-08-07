using System.Collections.Generic;
using System.Threading.Tasks;
using KidsQuiz.Data.Models;
using KidsQuiz.Services.DTOs.Kids;

namespace KidsQuiz.Services.Interfaces
{
    public interface IKidService
    {
        Task<KidDto> GetKidAsync(int id);
        Task<IEnumerable<KidDto>> GetAllKidsAsync();
        Task<KidDto> CreateKidAsync(KidCreateDto kidCreateDto);
        Task<KidDto> LoginKidAsync(string email, string name);
        Task UpdateKidAsync(int id, KidUpdateDto kidUpdateDto);
        Task DeleteKidAsync(int id);
        
        // Dynamic properties management
        Task UpdateKidIntroAsync(int id, string intro);
        Task UpdateKidAdditionalPropertiesAsync(int id, Dictionary<string, object> properties);
    }
} 