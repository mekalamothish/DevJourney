using DevJourney.Application.Dto.Articles;
using DevJourney.Application.Models;

namespace DevJourney.Application.Interfaces
{
    public interface IArticleService
    {
        Task<PagedResult<ArticleDto>> GetPagedAsync(ArticleQuery query);
        Task<ArticleDto> GetByIdAsync(int id);
        Task<ArticleDto> GetBySlugAsync(string slug);
        Task<ArticleDto> CreateAsync(ArticleCreateDto request);
        Task<ArticleDto> UpdateAsync(int id, ArticleUpdateDto request);
        Task<ArticleDto> PatchAsync(int id, DevJourney.Application.Dto.Articles.ArticlePatchDto patch);
        Task DeleteAsync(int id);
        Task<ArticleDto> PublishAsync(int id);
        Task<ArticleDto> UnpublishAsync(int id);
    }
}
