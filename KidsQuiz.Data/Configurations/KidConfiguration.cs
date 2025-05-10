using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using KidsQuiz.Data.Models;
using System.Text.Json;

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

            var property = builder.Property(k => k.DynamicProperties)
                   .HasColumnType("nvarchar(max)")
                   .HasConversion(
                       v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                       v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>()
                   );

            property.Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
                c => c.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            ));
        }
    }
}
