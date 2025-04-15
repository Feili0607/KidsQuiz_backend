using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KidsQuiz.Data.Models;

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
        }
    }
} 