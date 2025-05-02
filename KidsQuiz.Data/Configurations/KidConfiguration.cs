using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;
using System.Text.Json; // Add this

namespace KidsQuiz.Data.Configurations
{
    public class KidConfiguration : IEntityTypeConfiguration<Kid>
    {
        public void Configure(EntityTypeBuilder<Kid> builder)
        {
            builder.HasKey(k => k.Id);
            builder.Property(k => k.Name).IsRequired();
            builder.Property(k => k.Email).IsRequired();
            builder.Property(k => k.DateOfBirth).IsRequired();

            // 💡 Convert Dictionary<string, string> to JSON string in DB
            builder.Property(k => k.DynamicProperties)
                   .HasConversion(
                       v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                       v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null)
                   )
                   .HasColumnName("DynamicPropertiesJson") // Optional: name the DB column
                   .HasColumnType("nvarchar(max)"); // Optional: ensure EF creates a string column
        }
    }
}
