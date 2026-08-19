using DevJourney.Domain.Entities;
using DevJourney.Domain.Enums;
using DevJourney.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevJourney.Infrastructure.Persistence.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Slug)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(a => a.Slug).IsUnique();

        builder.Property(a => a.Excerpt)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.FeaturedImage)
            .HasMaxLength(2000);

        builder.Property(a => a.ReadingTime);

        // store enum as string matching contract values
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt).IsRequired();
        builder.Property(a => a.PublishedAt);
        builder.Property(a => a.IsFeatured).IsRequired();
        builder.Property(a => a.IsPopular).IsRequired();
        builder.Property(a => a.IsDeleted).IsRequired();
        builder.Property(a => a.DeletedAt);

        // Value converter between ArticleContent and string
        var converter = new ValueConverter<ArticleContent, string>(
            v => v.ToString(),
            s => new ArticleContent(s));

        builder.Property(a => a.Content)
            .HasConversion(converter)
            .HasColumnName("Content")
            .HasColumnType("TEXT")
            .IsRequired();

        // Indexes for common queries
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.PublishedAt);
        builder.HasIndex(a => a.CategoryId);
        builder.HasIndex(a => a.AuthorId);
        builder.HasIndex(a => a.IsDeleted);

        // Relationships
        builder.HasOne<DevJourney.Domain.Entities.Author>()
            .WithMany()
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DevJourney.Domain.Entities.Category>()
            .WithMany()
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ArticleTags relationship configured in ArticleTagConfiguration
    }
}
