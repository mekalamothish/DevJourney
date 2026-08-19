using DevJourney.Application.Dto.Common;
using DevJourney.Application.Exceptions;
using DevJourney.Application.Interfaces;
using DevJourney.Application.Models;
using DevJourney.Application.Repositories;
using DevJourney.Application.Validation;
using DevJourney.Application.Mapping;

namespace DevJourney.Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly TagValidator _validator;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
            _validator = new TagValidator(tagRepository);
        }

        public Task<PagedResult<TagDto>> GetPagedAsync(TagQuery query)
        {
            return _tagRepository.GetPagedAsync(query);
        }

        public async Task<TagDto> GetByIdAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) throw new NotFoundException("Tag", id);
            return tag;
        }

        public async Task<TagDto> CreateAsync(CreateTagDto request)
        {
            await _validator.ValidateCreateAsync(request.Name, request.Slug);

            if (string.IsNullOrWhiteSpace(request.Slug))
                request.Slug = SlugHelper.Normalize(request.Name);
            else
                request.Slug = SlugHelper.Normalize(request.Slug);

            var created = await _tagRepository.AddAsync(request);
            return created;
        }

        public async Task<TagDto> UpdateAsync(int id, UpdateTagDto request)
        {
            await _validator.ValidateUpdateAsync(id, request.Name, request.Slug);

            if (string.IsNullOrWhiteSpace(request.Slug))
                request.Slug = SlugHelper.Normalize(request.Name);
            else
                request.Slug = SlugHelper.Normalize(request.Slug);

            var updated = await _tagRepository.UpdateAsync(id, request);
            return updated;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _tagRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Tag", id);

            await _tagRepository.DeleteAsync(id);
        }
    }
}
