using DevJourney.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevJourney.Infrastructure.Persistence;

public class DevJourneyDbContext : DbContext
{
    public DevJourneyDbContext(DbContextOptions<DevJourneyDbContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<DevJourney.Domain.Entities.ArticleTag> ArticleTags => Set<DevJourney.Domain.Entities.ArticleTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevJourneyDbContext).Assembly);
    }
}
