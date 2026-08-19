using DevJourney.Application.Dto.Common;
using DevJourney.Application.Models;
using DevJourney.Application.Repositories;

namespace DevJourney.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetPagedAsync(CategoryQuery query);
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto request);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto request);
        Task DeleteAsync(int id);
    }
}
