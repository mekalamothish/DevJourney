using DevJourney.Application.Exceptions;
using DevJourney.Application.Repositories;

namespace DevJourney.Application.Validation
{
    public class TagValidator
    {
        private readonly ITagRepository _tagRepository;

        public TagValidator(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task ValidateCreateAsync(string name, string? slug)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("name", "Name is required");
            if (!string.IsNullOrEmpty(slug) && slug.Length > 255) throw new ValidationException("slug", "Slug must be 255 characters or less");

            if (!string.IsNullOrEmpty(slug))
            {
                var exists = await _tagRepository.SlugExistsAsync(slug);
                if (exists) throw new ConflictException("Tag slug already exists");
            }
        }

        public async Task ValidateUpdateAsync(int id, string name, string? slug)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("name", "Name is required");
            if (!string.IsNullOrEmpty(slug) && slug.Length > 255) throw new ValidationException("slug", "Slug must be 255 characters or less");

            var existing = await _tagRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Tag", id);

            if (!string.IsNullOrEmpty(slug))
            {
                var exists = await _tagRepository.SlugExistsAsync(slug);
                if (exists && !string.Equals(existing.Slug, slug, StringComparison.OrdinalIgnoreCase))
                    throw new ConflictException("Tag slug already exists");
            }
        }
    }
}
