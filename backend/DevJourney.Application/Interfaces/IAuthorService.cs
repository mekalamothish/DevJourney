using DevJourney.Application.Dto.Common;
using DevJourney.Application.Models;
using DevJourney.Application.Repositories;

namespace DevJourney.Application.Interfaces
{
    public interface IAuthorService
    {
        Task<PagedResult<AuthorDto>> GetPagedAsync(AuthorQuery query);
        Task<AuthorDto> GetByIdAsync(int id);
        Task<AuthorDto> CreateAsync(CreateAuthorDto request);
        Task<AuthorDto> UpdateAsync(int id, UpdateAuthorDto request);
    }
}
