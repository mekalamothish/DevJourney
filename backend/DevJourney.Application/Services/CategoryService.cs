using DevJourney.Application.Dto.Common;
using DevJourney.Application.Exceptions;
using DevJourney.Application.Interfaces;
using DevJourney.Application.Models;
using DevJourney.Application.Repositories;
using DevJourney.Application.Validation;
using DevJourney.Application.Mapping;

namespace DevJourney.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryValidator _validator;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _validator = new CategoryValidator(categoryRepository);
        }

        public Task<PagedResult<CategoryDto>> GetPagedAsync(CategoryQuery query)
        {
            return _categoryRepository.GetPagedAsync(query);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var cat = await _categoryRepository.GetByIdAsync(id);
            if (cat == null) throw new NotFoundException("Category", id);
            return cat;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto request)
        {
            await _validator.ValidateCreateAsync(request.Name, request.Slug);

            if (string.IsNullOrWhiteSpace(request.Slug))
                request.Slug = SlugHelper.Normalize(request.Name);
            else
                request.Slug = SlugHelper.Normalize(request.Slug);

            var created = await _categoryRepository.AddAsync(request);
            return created;
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto request)
        {
            await _validator.ValidateUpdateAsync(id, request.Name, request.Slug);

            if (string.IsNullOrWhiteSpace(request.Slug))
                request.Slug = SlugHelper.Normalize(request.Name);
            else
                request.Slug = SlugHelper.Normalize(request.Slug);

            var updated = await _categoryRepository.UpdateAsync(id, request);
            return updated;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Category", id);

            await _categoryRepository.DeleteAsync(id);
        }
    }
}
