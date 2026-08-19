namespace DevJourney.Infrastructure.Repositories
{
    using DevJourney.Application.Dto.Articles;
    using DevJourney.Application.Dto.Common;
    using DevJourney.Application.Models;
    using DevJourney.Application.Repositories;
    using DevJourney.Domain.Entities;
    using DevJourney.Domain.Enums;
    using DevJourney.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class ArticleRepository : IArticleRepository
    {
        private readonly DevJourneyDbContext _context;

        public ArticleRepository(DevJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<ArticleDto?> GetByIdAsync(int id)
        {
            var article = await _context.Articles
                .AsNoTracking()
                .Where(a => a.Id == id && !a.IsDeleted)
                .Include(a => a.ArticleTags)
                .FirstOrDefaultAsync();

            if (article == null)
                return null;

            return MapToDto(article);
        }

        public async Task<ArticleDto?> GetBySlugAsync(string slug)
        {
            var article = await _context.Articles
                .AsNoTracking()
                .Where(a => a.Slug == slug && !a.IsDeleted)
                .Include(a => a.ArticleTags)
                .FirstOrDefaultAsync();

            if (article == null)
                return null;

            return MapToDto(article);
        }

        public async Task<PagedResult<ArticleDto>> GetPagedAsync(ArticleQuery query)
        {
            // Validate pagination parameters
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Min(Math.Max(query.PageSize, 1), 100);

            var baseQuery = _context.Articles
                .AsNoTracking()
                .Where(a => !a.IsDeleted);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var searchTerm = query.Q.ToLower();
                baseQuery = baseQuery.Where(a => 
                    a.Title.ToLower().Contains(searchTerm) || 
                    a.Excerpt.ToLower().Contains(searchTerm));
            }

            if (query.AuthorId.HasValue)
            {
                baseQuery = baseQuery.Where(a => a.AuthorId == query.AuthorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                if (Enum.TryParse<ArticleStatus>(query.Status, ignoreCase: true, out var status))
                {
                    baseQuery = baseQuery.Where(a => a.Status == status);
                }
            }

            if (query.Since.HasValue)
            {
                baseQuery = baseQuery.Where(a => a.PublishedAt >= query.Since.Value);
            }

            if (query.Until.HasValue)
            {
                baseQuery = baseQuery.Where(a => a.PublishedAt <= query.Until.Value);
            }

            // Apply category filter
            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                if (int.TryParse(query.Category, out var categoryId))
                {
                    baseQuery = baseQuery.Where(a => a.CategoryId == categoryId);
                }
                else
                {
                    // Search by slug
                    baseQuery = baseQuery.Where(a => 
                        _context.Categories.Any(c => 
                            c.Id == a.CategoryId && c.Slug == query.Category));
                }
            }

            // Apply tag filter
            if (!string.IsNullOrWhiteSpace(query.Tag))
            {
                if (int.TryParse(query.Tag, out var tagId))
                {
                    baseQuery = baseQuery.Where(a => 
                        a.ArticleTags.Any(at => at.TagId == tagId));
                }
                else
                {
                    // Search by slug
                    baseQuery = baseQuery.Where(a => 
                        a.ArticleTags.Any(at => 
                            _context.Tags.Any(t => 
                                t.Id == at.TagId && t.Slug == query.Tag)));
                }
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(query.Sort))
            {
                baseQuery = query.Sort.ToLower() switch
                {
                    "publishedat" => baseQuery.OrderBy(a => a.PublishedAt),
                    "-publishedat" => baseQuery.OrderByDescending(a => a.PublishedAt),
                    "updatedat" => baseQuery.OrderBy(a => a.UpdatedAt),
                    "-updatedat" => baseQuery.OrderByDescending(a => a.UpdatedAt),
                    "createdat" => baseQuery.OrderBy(a => a.CreatedAt),
                    "-createdat" => baseQuery.OrderByDescending(a => a.CreatedAt),
                    _ => baseQuery.OrderByDescending(a => a.UpdatedAt) // Default
                };
            }
            else
            {
                // Default sort
                baseQuery = baseQuery.OrderByDescending(a => a.UpdatedAt);
            }

            // Count total before pagination
            var total = await baseQuery.CountAsync();

            // Apply pagination
            var articles = await baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.ArticleTags)
                .ToListAsync();

            var dtos = articles.Select(MapToDto).ToList();

            return new PagedResult<ArticleDto>
            {
                Data = dtos,
                Meta = new PaginationMetadata
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                }
            };
        }

        public async Task<ArticleDto> AddAsync(ArticleCreateDto createDto)
        {
            // Parse status
            var status = parseStatus(createDto.Status) ?? ArticleStatus.Draft;

            var article = Article.CreateNew(
                id: 0, // EF will generate the ID
                title: createDto.Title,
                slug: createDto.Slug ?? GenerateSlugFromTitle(createDto.Title),
                excerpt: createDto.Excerpt,
                authorId: createDto.AuthorId,
                categoryId: createDto.CategoryId,
                content: new DevJourney.Domain.ValueObjects.ArticleContent(
                    System.Text.Json.JsonSerializer.Serialize(createDto.Content)),
                nowUtc: DateTime.UtcNow,
                featuredImage: createDto.FeaturedImage,
                readingTime: createDto.ReadingTime
            );

            // Set status
            if (status == ArticleStatus.Published)
            {
                article.Publish(createDto.PublishedAt ?? DateTime.UtcNow);
            }

            _context.Articles.Add(article);

            // Add tags
            if (createDto.TagIds?.Any() == true)
            {
                foreach (var tagId in createDto.TagIds)
                {
                    article.AddTag(tagId);
                }
            }

            await _context.SaveChangesAsync();

            return MapToDto(article);
        }

        public async Task<ArticleDto> UpdateAsync(int id, ArticleUpdateDto updateDto)
        {
            var article = await _context.Articles
                .Include(a => a.ArticleTags)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                throw new KeyNotFoundException($"Article with ID {id} not found.");

            // Update metadata
            article.UpdateMetadata(
                title: updateDto.Title,
                slug: updateDto.Slug ?? GenerateSlugFromTitle(updateDto.Title),
                excerpt: updateDto.Excerpt,
                authorId: updateDto.AuthorId,
                categoryId: updateDto.CategoryId,
                featuredImage: updateDto.FeaturedImage,
                readingTime: updateDto.ReadingTime,
                isFeatured: false, // Not exposed in update for now
                isPopular: false, // Not exposed in update for now
                nowUtc: DateTime.UtcNow
            );

            // Update content
            article.UpdateContent(
                new DevJourney.Domain.ValueObjects.ArticleContent(
                    System.Text.Json.JsonSerializer.Serialize(updateDto.Content)),
                DateTime.UtcNow
            );

            // Update status if provided
            if (!string.IsNullOrWhiteSpace(updateDto.Status))
            {
                var status = parseStatus(updateDto.Status);
                if (status.HasValue && status != article.Status)
                {
                    switch (status)
                    {
                        case ArticleStatus.Published:
                            article.Publish(updateDto.PublishedAt ?? DateTime.UtcNow);
                            break;
                        case ArticleStatus.Draft:
                            article.Unpublish(DateTime.UtcNow);
                            break;
                        case ArticleStatus.Archived:
                            article.Archive(DateTime.UtcNow);
                            break;
                    }
                }
            }

            // Update tags
            if (updateDto.TagIds != null)
            {
                // Remove old tags
                foreach (var oldTag in article.ArticleTags.ToList())
                {
                    article.RemoveTag(oldTag.TagId);
                }

                // Add new tags
                foreach (var tagId in updateDto.TagIds)
                {
                    article.AddTag(tagId);
                }
            }

            _context.Articles.Update(article);
            await _context.SaveChangesAsync();

            return MapToDto(article);
        }

        public async Task DeleteAsync(int id)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                throw new KeyNotFoundException($"Article with ID {id} not found.");

            article.MarkDeleted(DateTime.UtcNow);
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Articles
                .Where(a => !a.IsDeleted)
                .AnyAsync(a => a.Slug == slug);
        }

        public async Task<ArticleDto> PublishAsync(int id)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                throw new KeyNotFoundException($"Article with ID {id} not found.");

            article.Publish(DateTime.UtcNow);
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();

            return MapToDto(article);
        }

        public async Task<ArticleDto> UnpublishAsync(int id)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
                throw new KeyNotFoundException($"Article with ID {id} not found.");

            article.Unpublish(DateTime.UtcNow);
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();

            return MapToDto(article);
        }

        private ArticleDto MapToDto(Article article)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == article.AuthorId);
            var category = _context.Categories.FirstOrDefault(c => c.Id == article.CategoryId);
            var tags = _context.ArticleTags
                .Where(at => at.ArticleId == article.Id)
                .Select(at => _context.Tags.FirstOrDefault(t => t.Id == at.TagId))
                .Where(t => t != null)
                .ToList();

            var contentJson = article.Content.ToString();
            var contentBlocks = System.Text.Json.JsonSerializer.Deserialize<List<DevJourney.Application.ContentBlocks.ArticleBlock>>(contentJson)
                ?? new List<DevJourney.Application.ContentBlocks.ArticleBlock>();

            return new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Excerpt = article.Excerpt,
                FeaturedImage = article.FeaturedImage,
                ReadingTime = article.ReadingTime,
                Status = article.Status.ToString().ToLower(),
                CreatedAt = DateTime.SpecifyKind(article.CreatedAt, DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(article.UpdatedAt, DateTimeKind.Utc),
                PublishedAt = article.PublishedAt.HasValue ? DateTime.SpecifyKind(article.PublishedAt.Value, DateTimeKind.Utc) : null,
                Author = new AuthorDto
                {
                    Id = author?.Id ?? 0,
                    Name = author?.Name ?? "Unknown",
                    Avatar = author?.Avatar,
                    Role = author?.Role
                },
                Category = new CategoryDto
                {
                    Id = category?.Id ?? 0,
                    Name = category?.Name ?? "Unknown",
                    Slug = category?.Slug ?? ""
                },
                Tags = tags.Where(t => t != null).Select(t => new TagDto
                {
                    Id = t!.Id,
                    Name = t!.Name,
                    Slug = t!.Slug
                }).ToList(),
                Content = contentBlocks,
                IsFeatured = article.IsFeatured,
                IsPopular = article.IsPopular
            };
        }

        private static ArticleStatus? parseStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            return status.ToLower() switch
            {
                "draft" => ArticleStatus.Draft,
                "published" => ArticleStatus.Published,
                "archived" => ArticleStatus.Archived,
                _ => null
            };
        }

        private static string GenerateSlugFromTitle(string title)
        {
            return title
                .ToLower()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("/", "-")
                .Replace("\\", "-");
        }
    }
}
