namespace DevJourney.Infrastructure.Repositories
{
    using DevJourney.Application.Dto.Common;
    using DevJourney.Application.Models;
    using DevJourney.Application.Repositories;
    using DevJourney.Domain.Entities;
    using DevJourney.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly DevJourneyDbContext _context;

        public CategoryRepository(DevJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return null;

            return MapToDto(category);
        }

        public async Task<PagedResult<CategoryDto>> GetPagedAsync(CategoryQuery query)
        {
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Min(Math.Max(query.PageSize, 1), 100);

            var total = await _context.Categories.CountAsync();

            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Data = categories.Select(MapToDto).ToList(),
                Meta = new PaginationMetadata
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                }
            };
        }

        public async Task<CategoryDto> AddAsync(CreateCategoryDto createDto)
        {
            var category = new Category(0, createDto.Name, createDto.Slug);
            
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto updateDto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            category.Update(updateDto.Name, updateDto.Slug);

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Categories
                .AnyAsync(c => c.Slug == slug);
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug
            };
        }
    }
}
