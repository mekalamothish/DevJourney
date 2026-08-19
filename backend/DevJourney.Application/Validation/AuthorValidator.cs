using DevJourney.Application.Exceptions;
using DevJourney.Application.Repositories;

namespace DevJourney.Application.Validation
{
    public class AuthorValidator
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorValidator(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task ValidateCreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("name", "Name is required");
        }

        public async Task ValidateUpdateAsync(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("name", "Name is required");

            var existing = await _authorRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Author", id);
        }
    }
}
