using DevJourney.Application.ContentBlocks;
using DevJourney.Application.Dto.Articles;
using DevJourney.Application.Exceptions;
using DevJourney.Application.Repositories;

namespace DevJourney.Application.Validation
{
    public class ArticleValidator
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;

        public ArticleValidator(IAuthorRepository authorRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository)
        {
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
        }

        public async Task ValidateCreateAsync(ArticleCreateDto dto)
        {
            if (dto == null) throw new ValidationException("request", "Article create request is required");
            ValidateCommon(dto.Title, dto.Slug, dto.Excerpt, dto.Content);

            // author
            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null) throw new ValidationException("authorId", "Author not found");

            // category
            var cat = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (cat == null) throw new ValidationException("categoryId", "Category not found");

            // tags
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                foreach (var id in dto.TagIds)
                {
                    var tag = await _tagRepository.GetByIdAsync(id);
                    if (tag == null) throw new ValidationException("tagIds", $"Tag {id} not found");
                }
            }
        }

        public async Task ValidateUpdateAsync(ArticleUpdateDto dto)
        {
            if (dto == null) throw new ValidationException("request", "Article update request is required");
            ValidateCommon(dto.Title, dto.Slug, dto.Excerpt, dto.Content);

            var author = await _authorRepository.GetByIdAsync(dto.AuthorId);
            if (author == null) throw new ValidationException("authorId", "Author not found");

            var cat = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (cat == null) throw new ValidationException("categoryId", "Category not found");

            if (dto.TagIds != null && dto.TagIds.Any())
            {
                foreach (var id in dto.TagIds)
                {
                    var tag = await _tagRepository.GetByIdAsync(id);
                    if (tag == null) throw new ValidationException("tagIds", $"Tag {id} not found");
                }
            }
        }

        private void ValidateCommon(string title, string? slug, string excerpt, IEnumerable<ArticleBlock> content)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ValidationException("title", "Title is required");
            if (title.Length > 255) throw new ValidationException("title", "Title must be 255 characters or less");

            if (!string.IsNullOrEmpty(slug) && slug.Length > 255) throw new ValidationException("slug", "Slug must be 255 characters or less");

            if (string.IsNullOrWhiteSpace(excerpt)) throw new ValidationException("excerpt", "Excerpt is required");
            if (excerpt.Length > 1000) throw new ValidationException("excerpt", "Excerpt must be 1000 characters or less");

            if (content == null || !content.Any()) throw new ValidationException("content", "Content must contain at least one block");
            ArticleBlockValidator.ValidateBlocks(content);
        }
    }
}
