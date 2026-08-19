namespace DevJourney.Application.Repositories
{
    using DevJourney.Application.Dto.Common;
    using DevJourney.Application.Models;

    /// <summary>
    /// Repository interface for Author persistence operations.
    /// </summary>
    public interface IAuthorRepository
    {
        /// <summary>
        /// Get author by ID.
        /// </summary>
        Task<AuthorDto?> GetByIdAsync(int id);

        /// <summary>
        /// Get paged list of authors.
        /// </summary>
        Task<PagedResult<AuthorDto>> GetPagedAsync(int page = 1, int pageSize = 20);

        /// <summary>
        /// Add a new author.
        /// </summary>
        Task<AuthorDto> AddAsync(CreateAuthorDto createDto);

        /// <summary>
        /// Update an existing author.
        /// </summary>
        Task<AuthorDto> UpdateAsync(int id, UpdateAuthorDto updateDto);
    }

    /// <summary>
    /// DTO for creating an author.
    /// </summary>
    public class CreateAuthorDto
    {
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? Role { get; set; }
        public string? Bio { get; set; }
    }

    /// <summary>
    /// DTO for updating an author.
    /// </summary>
    public class UpdateAuthorDto
    {
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? Role { get; set; }
        public string? Bio { get; set; }
    }
}
