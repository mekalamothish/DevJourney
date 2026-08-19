namespace DevJourney.Application.Repositories
{
    using DevJourney.Application.Dto.Common;
    using DevJourney.Application.Models;

    /// <summary>
    /// Repository interface for Category persistence operations.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Get category by ID.
        /// </summary>
        Task<CategoryDto?> GetByIdAsync(int id);

        /// <summary>
        /// Get paged list of categories.
        /// </summary>
        Task<PagedResult<CategoryDto>> GetPagedAsync(CategoryQuery query);

        /// <summary>
        /// Add a new category.
        /// </summary>
        Task<CategoryDto> AddAsync(CreateCategoryDto createDto);

        /// <summary>
        /// Update an existing category.
        /// </summary>
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateDto);

        /// <summary>
        /// Delete a category.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Check if slug already exists.
        /// </summary>
        Task<bool> SlugExistsAsync(string slug);
    }

    /// <summary>
    /// DTO for creating a category.
    /// </summary>
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    /// <summary>
    /// DTO for updating a category.
    /// </summary>
    public class UpdateCategoryDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
