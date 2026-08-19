using DevJourney.Application.Dto.Common;
using DevJourney.Application.Exceptions;
using DevJourney.Application.Interfaces;
using DevJourney.Application.Models;
using DevJourney.Application.Repositories;
using DevJourney.Application.Validation;

namespace DevJourney.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly AuthorValidator _validator;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
            _validator = new AuthorValidator(authorRepository);
        }

        public Task<PagedResult<AuthorDto>> GetPagedAsync(AuthorQuery query)
        {
            // repository signature uses page/pageSize but adapter exists in repo; keep simple
            return _authorRepository.GetPagedAsync(query.Page, query.PageSize);
        }

        public async Task<AuthorDto> GetByIdAsync(int id)
        {
            var a = await _authorRepository.GetByIdAsync(id);
            if (a == null) throw new NotFoundException("Author", id);
            return a;
        }

        public async Task<AuthorDto> CreateAsync(CreateAuthorDto request)
        {
            await _validator.ValidateCreateAsync(request.Name);
            var created = await _authorRepository.AddAsync(request);
            return created;
        }

        public async Task<AuthorDto> UpdateAsync(int id, UpdateAuthorDto request)
        {
            await _validator.ValidateUpdateAsync(id, request.Name);
            var updated = await _authorRepository.UpdateAsync(id, request);
            return updated;
        }
    }
}
