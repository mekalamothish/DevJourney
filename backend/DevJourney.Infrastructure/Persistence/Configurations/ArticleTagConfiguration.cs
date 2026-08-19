using DevJourney.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevJourney.Infrastructure.Persistence.Configurations;

public class ArticleTagConfiguration : IEntityTypeConfiguration<ArticleTag>
{
    public void Configure(EntityTypeBuilder<ArticleTag> builder)
    {
        builder.ToTable("ArticleTags");
        builder.HasKey(at => new { at.ArticleId, at.TagId });

        builder.HasOne<Article>()
            .WithMany(a => a.ArticleTags)
            .HasForeignKey(at => at.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(at => at.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
