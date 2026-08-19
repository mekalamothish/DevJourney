using DevJourney.Application.Dto.Common;
using DevJourney.Application.Models;
using DevJourney.Application.Repositories;

namespace DevJourney.Application.Interfaces
{
    public interface ITagService
    {
        Task<PagedResult<TagDto>> GetPagedAsync(TagQuery query);
        Task<TagDto> GetByIdAsync(int id);
        Task<TagDto> CreateAsync(CreateTagDto request);
        Task<TagDto> UpdateAsync(int id, UpdateTagDto request);
        Task DeleteAsync(int id);
    }
}
