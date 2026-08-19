namespace DevJourney.Infrastructure.Repositories
{
    using DevJourney.Application.Dto.Common;
    using DevJourney.Application.Models;
    using DevJourney.Application.Repositories;
    using DevJourney.Domain.Entities;
    using DevJourney.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class AuthorRepository : IAuthorRepository
    {
        private readonly DevJourneyDbContext _context;

        public AuthorRepository(DevJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorDto?> GetByIdAsync(int id)
        {
            var author = await _context.Authors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return null;

            return MapToDto(author);
        }

        public async Task<PagedResult<AuthorDto>> GetPagedAsync(int page = 1, int pageSize = 20)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Min(Math.Max(pageSize, 1), 100);

            var total = await _context.Authors.CountAsync();

            var authors = await _context.Authors
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AuthorDto>
            {
                Data = authors.Select(MapToDto).ToList(),
                Meta = new PaginationMetadata
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                }
            };
        }

        public async Task<AuthorDto> AddAsync(CreateAuthorDto createDto)
        {
            var author = new Author(0, createDto.Name, createDto.Avatar, createDto.Role, createDto.Bio);
            
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return MapToDto(author);
        }

        public async Task<AuthorDto> UpdateAsync(int id, UpdateAuthorDto updateDto)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                throw new KeyNotFoundException($"Author with ID {id} not found.");

            author.UpdateProfile(updateDto.Name, updateDto.Avatar, updateDto.Role, updateDto.Bio);

            _context.Authors.Update(author);
            await _context.SaveChangesAsync();

            return MapToDto(author);
        }

        private static AuthorDto MapToDto(Author author)
        {
            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Avatar = author.Avatar,
                Role = author.Role
            };
        }
    }
}
