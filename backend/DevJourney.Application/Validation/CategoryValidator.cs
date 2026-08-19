using DevJourney.Application.Exceptions;
using DevJourney.Application.Repositories;

namespace DevJourney.Application.Validation
{
    public class CategoryValidator
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task ValidateCreateAsync(string name, string? slug)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("name", "Name is required");
            if (!string.IsNullOrEmpty(slug) && slug.Length > 255) throw new ValidationException("slug", "Slug must be 255 characters or less");

            if (!string.IsNullOrEmpty(slug))
            {
                var exists = await _categoryRepository.SlugExistsAsync(slug);
                if (exists) throw new ConflictException("Category slug already exists");
            }
        }

        public async Task ValidateUpdateAsync(int id, string name, string? slug)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("name", "Name is required");
            if (!string.IsNullOrEmpty(slug) && slug.Length > 255) throw new ValidationException("slug", "Slug must be 255 characters or less");

            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Category", id);

            if (!string.IsNullOrEmpty(slug))
            {
                var exists = await _categoryRepository.SlugExistsAsync(slug);
                if (exists && !string.Equals(existing.Slug, slug, StringComparison.OrdinalIgnoreCase))
                    throw new ConflictException("Category slug already exists");
            }
        }
    }
}
