using DevJourney.Application.Dto.Articles;
using DevJourney.Application.Exceptions;
using DevJourney.Application.Interfaces;
using DevJourney.Application.Models;
using DevJourney.Application.Repositories;
using DevJourney.Application.Validation;
using DevJourney.Application.Mapping;

namespace DevJourney.Application.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ArticleValidator _validator;

        public ArticleService(
            IArticleRepository articleRepository,
            IAuthorRepository authorRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository)
        {
            _articleRepository = articleRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _validator = new ArticleValidator(authorRepository, categoryRepository, tagRepository);
        }

        public Task<PagedResult<ArticleDto>> GetPagedAsync(ArticleQuery query)
        {
            return _articleRepository.GetPagedAsync(query);
        }

        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null) throw new NotFoundException("Article", id);
            return article;
        }

        public async Task<ArticleDto> GetBySlugAsync(string slug)
        {
            var normalized = SlugHelper.Normalize(slug);
            var article = await _articleRepository.GetBySlugAsync(normalized);
            if (article == null) throw new NotFoundException("Article", slug);
            return article;
        }

        public async Task<ArticleDto> CreateAsync(ArticleCreateDto request)
        {
            await _validator.ValidateCreateAsync(request);

            // slug generation/normalization
            if (string.IsNullOrWhiteSpace(request.Slug))
                request.Slug = SlugHelper.Normalize(request.Title);
            else
                request.Slug = SlugHelper.Normalize(request.Slug);

            if (string.IsNullOrWhiteSpace(request.Slug))
                throw new ValidationException("slug", "Slug could not be generated or is empty");

            var exists = await _articleRepository.SlugExistsAsync(request.Slug);
            if (exists) throw new ConflictException("Article slug already exists");

            // Calculate reading time (simple heuristic)
            request.ReadingTime = CalculateReadingTime(request.Content);

            var created = await _articleRepository.AddAsync(request);
            return created;
        }

        public async Task<ArticleDto> UpdateAsync(int id, ArticleUpdateDto request)
        {
            await _validator.ValidateUpdateAsync(request);

            var existing = await _articleRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Article", id);

            if (string.IsNullOrWhiteSpace(request.Slug))
                request.Slug = SlugHelper.Normalize(request.Title);
            else
                request.Slug = SlugHelper.Normalize(request.Slug);

            if (string.IsNullOrWhiteSpace(request.Slug))
                throw new ValidationException("slug", "Slug could not be generated or is empty");

            var slugExists = await _articleRepository.SlugExistsAsync(request.Slug);
            if (slugExists && !string.Equals(existing.Slug, request.Slug, StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Article slug already exists");

            request.ReadingTime = CalculateReadingTime(request.Content);

            var updated = await _articleRepository.UpdateAsync(id, request);
            return updated;
        }

        public async Task<ArticleDto> PatchAsync(int id, DevJourney.Application.Dto.Articles.ArticlePatchDto patch)
        {
            var existing = await _articleRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Article", id);

            // Build merged update DTO from existing values
            var merged = new ArticleUpdateDto
            {
                Title = patch.TitleProvided ? patch.Title : existing.Title,
                Slug = patch.SlugProvided ? patch.Slug : existing.Slug,
                Excerpt = patch.ExcerptProvided ? patch.Excerpt : existing.Excerpt,
                FeaturedImage = patch.FeaturedImageProvided ? patch.FeaturedImage : existing.FeaturedImage,
                ReadingTime = patch.ReadingTimeProvided ? patch.ReadingTime : existing.ReadingTime,
                Status = patch.StatusProvided ? patch.Status : existing.Status,
                PublishedAt = patch.PublishedAtProvided ? patch.PublishedAt : existing.PublishedAt,
                AuthorId = patch.AuthorIdProvided ? patch.AuthorId ?? existing.Author.Id : existing.Author.Id,
                CategoryId = patch.CategoryIdProvided ? patch.CategoryId ?? existing.Category.Id : existing.Category.Id,
                TagIds = patch.TagIdsProvided ? patch.TagIds ?? new List<int>() : existing.Tags?.Select(t => t.Id).ToList() ?? new List<int>(),
                Content = patch.ContentProvided ? patch.Content ?? new List<DevJourney.Application.ContentBlocks.ArticleBlock>() : existing.Content ?? new List<DevJourney.Application.ContentBlocks.ArticleBlock>()
            };

            // Validate merged DTO
            await _validator.ValidateUpdateAsync(merged);

            // Slug normalization and uniqueness check
            if (string.IsNullOrWhiteSpace(merged.Slug))
                merged.Slug = SlugHelper.Normalize(merged.Title);
            else
                merged.Slug = SlugHelper.Normalize(merged.Slug);

            if (string.IsNullOrWhiteSpace(merged.Slug))
                throw new ValidationException("slug", "Slug could not be generated or is empty");

            var slugExists = await _articleRepository.SlugExistsAsync(merged.Slug);
            if (slugExists && !string.Equals(existing.Slug, merged.Slug, StringComparison.OrdinalIgnoreCase))
                throw new ConflictException("Article slug already exists");

            // Recalculate reading time if content provided
            if (patch.ContentProvided)
            {
                merged.ReadingTime = CalculateReadingTime(merged.Content);
            }

            var updated = await _articleRepository.UpdateAsync(id, merged);
            return updated;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _articleRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Article", id);

            await _articleRepository.DeleteAsync(id);
        }

        public async Task<ArticleDto> PublishAsync(int id)
        {
            var existing = await _articleRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Article", id);

            var published = await _articleRepository.PublishAsync(id);
            return published;
        }

        public async Task<ArticleDto> UnpublishAsync(int id)
        {
            var existing = await _articleRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException("Article", id);

            var result = await _articleRepository.UnpublishAsync(id);
            return result;
        }

        private int CalculateReadingTime(IEnumerable<DevJourney.Application.ContentBlocks.ArticleBlock> content)
        {
            if (content == null) return 1;
            // simple heuristic: count words in text-like blocks and divide by 200 wpm
            int words = 0;
            foreach (var block in content)
            {
                switch (block)
                {
                    case DevJourney.Application.ContentBlocks.ParagraphBlock p:
                        words += CountWords(p.Text);
                        break;
                    case DevJourney.Application.ContentBlocks.HeadingBlock h:
                        words += CountWords(h.Text);
                        break;
                    case DevJourney.Application.ContentBlocks.SubheadingBlock s:
                        words += CountWords(s.Text);
                        break;
                    case DevJourney.Application.ContentBlocks.ListBlock l:
                        foreach (var item in l.Items) words += CountWords(item);
                        break;
                    case DevJourney.Application.ContentBlocks.CodeBlock c:
                        words += CountWords(c.Code) / 5; // code counts less
                        break;
                    case DevJourney.Application.ContentBlocks.QuoteBlock q:
                        words += CountWords(q.Text);
                        break;
                    case DevJourney.Application.ContentBlocks.CalloutBlock co:
                        words += CountWords(co.Text);
                        break;
                    case DevJourney.Application.ContentBlocks.TakeawaysBlock tk:
                        foreach (var item in tk.Items) words += CountWords(item);
                        break;
                    case DevJourney.Application.ContentBlocks.FaqBlock fq:
                        foreach (var it in fq.Items)
                        {
                            words += CountWords(it.Question) + CountWords(it.Answer);
                        }
                        break;
                }
            }

            var minutes = Math.Max(1, (int)Math.Ceiling(words / 200.0));
            return minutes;
        }

        private int CountWords(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            var parts = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length;
        }
    }
}
