namespace DevJourney.Application.Repositories
{
    using DevJourney.Application.Dto.Common;
    using DevJourney.Application.Models;

    /// <summary>
    /// Repository interface for Tag persistence operations.
    /// </summary>
    public interface ITagRepository
    {
        /// <summary>
        /// Get tag by ID.
        /// </summary>
        Task<TagDto?> GetByIdAsync(int id);

        /// <summary>
        /// Get paged list of tags with optional search.
        /// </summary>
        Task<PagedResult<TagDto>> GetPagedAsync(TagQuery query);

        /// <summary>
        /// Add a new tag.
        /// </summary>
        Task<TagDto> AddAsync(CreateTagDto createDto);

        /// <summary>
        /// Update an existing tag.
        /// </summary>
        Task<TagDto> UpdateAsync(int id, UpdateTagDto updateDto);

        /// <summary>
        /// Delete a tag.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Check if slug already exists.
        /// </summary>
        Task<bool> SlugExistsAsync(string slug);
    }

    /// <summary>
    /// DTO for creating a tag.
    /// </summary>
    public class CreateTagDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    /// <summary>
    /// DTO for updating a tag.
    /// </summary>
    public class UpdateTagDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
