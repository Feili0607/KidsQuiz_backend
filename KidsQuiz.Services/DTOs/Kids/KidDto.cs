using System;
using System.Collections.Generic;
using System.Text.Json;

namespace KidsQuiz.Services.DTOs.Kids
{
    public class KidDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Intro { get; set; }
        public string Grade { get; set; }

        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public Dictionary<string, object> AdditionalProperties { get; set; }

        // Helper methods for common properties
        public T GetProperty<T>(string key, T defaultValue = default)
        {
            if (Properties.TryGetValue(key, out var value))
            {
                if (value is JsonElement jsonElement)
                {
                    return jsonElement.Deserialize<T>();
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return defaultValue;
        }

        public void SetProperty<T>(string key, T value)
        {
            Properties[key] = value;
        }

        public bool HasProperty(string key)
        {
            return Properties.ContainsKey(key);
        }

        public void RemoveProperty(string key)
        {
            Properties.Remove(key);
        }
    }
} 