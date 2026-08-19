namespace DevJourney.Infrastructure.Repositories
{
    using DevJourney.Application.Dto.Common;
    using DevJourney.Application.Models;
    using DevJourney.Application.Repositories;
    using DevJourney.Domain.Entities;
    using DevJourney.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class TagRepository : ITagRepository
    {
        private readonly DevJourneyDbContext _context;

        public TagRepository(DevJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<TagDto?> GetByIdAsync(int id)
        {
            var tag = await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
                return null;

            return MapToDto(tag);
        }

        public async Task<PagedResult<TagDto>> GetPagedAsync(TagQuery query)
        {
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Min(Math.Max(query.PageSize, 1), 100);

            var baseQuery = _context.Tags.AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var searchTerm = query.Q.ToLower();
                baseQuery = baseQuery.Where(t => 
                    t.Name.ToLower().Contains(searchTerm) || 
                    t.Slug.ToLower().Contains(searchTerm));
            }

            var total = await baseQuery.CountAsync();

            var tags = await baseQuery
                .OrderBy(t => t.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TagDto>
            {
                Data = tags.Select(MapToDto).ToList(),
                Meta = new PaginationMetadata
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                }
            };
        }

        public async Task<TagDto> AddAsync(CreateTagDto createDto)
        {
            var tag = new Tag(0, createDto.Name, createDto.Slug);
            
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return MapToDto(tag);
        }

        public async Task<TagDto> UpdateAsync(int id, UpdateTagDto updateDto)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {id} not found.");

            tag.Update(updateDto.Name, updateDto.Slug);

            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();

            return MapToDto(tag);
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {id} not found.");

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Tags
                .AnyAsync(t => t.Slug == slug);
        }

        private static TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Slug = tag.Slug
            };
        }
    }
}
