namespace DevJourney.Application.Repositories
{
    using DevJourney.Application.Dto.Articles;
    using DevJourney.Application.Models;

    /// <summary>
    /// Repository interface for Article persistence operations.
    /// </summary>
    public interface IArticleRepository
    {
        /// <summary>
        /// Get article by ID.
        /// </summary>
        Task<ArticleDto?> GetByIdAsync(int id);

        /// <summary>
        /// Get article by slug.
        /// </summary>
        Task<ArticleDto?> GetBySlugAsync(string slug);

        /// <summary>
        /// Get paged list of articles using query filters.
        /// </summary>
        Task<PagedResult<ArticleDto>> GetPagedAsync(ArticleQuery query);

        /// <summary>
        /// Add a new article.
        /// </summary>
        Task<ArticleDto> AddAsync(ArticleCreateDto createDto);

        /// <summary>
        /// Update an existing article.
        /// </summary>
        Task<ArticleDto> UpdateAsync(int id, ArticleUpdateDto updateDto);

        /// <summary>
        /// Delete an article.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Check if slug already exists.
        /// </summary>
        Task<bool> SlugExistsAsync(string slug);

        /// <summary>
        /// Publish an article (set status to published).
        /// </summary>
        Task<ArticleDto> PublishAsync(int id);

        /// <summary>
        /// Unpublish an article (set status to draft).
        /// </summary>
        Task<ArticleDto> UnpublishAsync(int id);
    }
}
