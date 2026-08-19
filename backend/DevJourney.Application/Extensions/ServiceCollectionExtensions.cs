using Microsoft.Extensions.DependencyInjection;
using DevJourney.Application.Interfaces;
using DevJourney.Application.Services;

namespace DevJourney.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IAuthorService, AuthorService>();
            return services;
        }
    }
}
