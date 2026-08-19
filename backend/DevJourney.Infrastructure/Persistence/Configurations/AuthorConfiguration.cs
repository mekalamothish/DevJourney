using DevJourney.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevJourney.Infrastructure.Persistence.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(255);
        builder.Property(a => a.Avatar).HasMaxLength(2000);
        builder.Property(a => a.Role).HasMaxLength(255);
        builder.Property(a => a.Bio).HasMaxLength(2000);
    }
}
