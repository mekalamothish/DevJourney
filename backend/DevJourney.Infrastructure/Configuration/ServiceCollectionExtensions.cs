using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DevJourney.Application.Repositories;
using DevJourney.Infrastructure.Persistence;
using DevJourney.Infrastructure.Repositories;

namespace DevJourney.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<DevJourneyDbContext>(options =>
        {
            options.UseSqlite(conn);
        });

        // Register repositories
        services.AddScoped<IArticleRepository, ArticleRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        return services;
    }
}
