using System;
using System.Collections.Generic;
using DevJourney.Domain.Common;
using DevJourney.Domain.Enums;
using DevJourney.Domain.ValueObjects;

namespace DevJourney.Domain.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; private set; }
        public string Slug { get; private set; }
        public string Excerpt { get; private set; }
        public string? FeaturedImage { get; private set; }
        public int? ReadingTime { get; private set; }
        public ArticleStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public int AuthorId { get; private set; }
        public int CategoryId { get; private set; }
        public ArticleContent Content { get; private set; }
        public bool IsFeatured { get; private set; }
        public bool IsPopular { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        public IReadOnlyCollection<ArticleTag> ArticleTags => _articleTags.AsReadOnly();
        private readonly List<ArticleTag> _articleTags = new();

        private Article(int id,
                        string title,
                        string slug,
                        string excerpt,
                        int authorId,
                        int categoryId,
                        ArticleContent content,
                        string? featuredImage,
                        int? readingTime,
                        ArticleStatus status,
                        DateTime createdAt,
                        DateTime updatedAt,
                        DateTime? publishedAt,
                        bool isFeatured,
                        bool isPopular,
                        bool isDeleted,
                        DateTime? deletedAt)
        {
            Id = id;
            Title = title;
            Slug = slug;
            Excerpt = excerpt;
            AuthorId = authorId;
            CategoryId = categoryId;
            Content = content;
            FeaturedImage = featuredImage;
            ReadingTime = readingTime;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            PublishedAt = publishedAt;
            IsFeatured = isFeatured;
            IsPopular = isPopular;
            IsDeleted = isDeleted;
            DeletedAt = deletedAt;
        }

        public static Article CreateNew(int id,
                                        string title,
                                        string slug,
                                        string excerpt,
                                        int authorId,
                                        int categoryId,
                                        ArticleContent content,
                                        DateTime nowUtc,
                                        string? featuredImage = null,
                                        int? readingTime = null)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required", nameof(title));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug is required", nameof(slug));
            if (string.IsNullOrWhiteSpace(excerpt)) throw new ArgumentException("Excerpt is required", nameof(excerpt));
            if (authorId <= 0) throw new ArgumentException("AuthorId is required", nameof(authorId));
            if (categoryId <= 0) throw new ArgumentException("CategoryId is required", nameof(categoryId));
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (content.IsEmpty()) throw new ArgumentException("Content cannot be empty", nameof(content));

            return new Article(
                id: id,
                title: title,
                slug: slug,
                excerpt: excerpt,
                authorId: authorId,
                categoryId: categoryId,
                content: content,
                featuredImage: featuredImage,
                readingTime: readingTime,
                status: ArticleStatus.Draft,
                createdAt: nowUtc,
                updatedAt: nowUtc,
                publishedAt: null,
                isFeatured: false,
                isPopular: false,
                isDeleted: false,
                deletedAt: null
            );
        }

        public void Publish(DateTime nowUtc)
        {
            Status = ArticleStatus.Published;
            if (PublishedAt == null) PublishedAt = nowUtc;
            UpdatedAt = nowUtc;
        }

        public void Unpublish(DateTime nowUtc)
        {
            Status = ArticleStatus.Draft;
            UpdatedAt = nowUtc;
        }

        public void Archive(DateTime nowUtc)
        {
            Status = ArticleStatus.Archived;
            UpdatedAt = nowUtc;
        }

        public void UpdateContent(ArticleContent content, DateTime nowUtc)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (content.IsEmpty()) throw new ArgumentException("Content cannot be empty", nameof(content));
            Content = content;
            UpdatedAt = nowUtc;
        }

        public void UpdateMetadata(string title,
                                   string slug,
                                   string excerpt,
                                   int authorId,
                                   int categoryId,
                                   string? featuredImage,
                                   int? readingTime,
                                   bool isFeatured,
                                   bool isPopular,
                                   DateTime nowUtc)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required", nameof(title));
            if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug is required", nameof(slug));
            if (string.IsNullOrWhiteSpace(excerpt)) throw new ArgumentException("Excerpt is required", nameof(excerpt));
            if (authorId <= 0) throw new ArgumentException("AuthorId is required", nameof(authorId));
            if (categoryId <= 0) throw new ArgumentException("CategoryId is required", nameof(categoryId));

            Title = title;
            Slug = slug;
            Excerpt = excerpt;
            AuthorId = authorId;
            CategoryId = categoryId;
            FeaturedImage = featuredImage;
            ReadingTime = readingTime;
            IsFeatured = isFeatured;
            IsPopular = isPopular;
            UpdatedAt = nowUtc;
        }

        public void AddTag(int tagId)
        {
            if (tagId <= 0) throw new ArgumentException("TagId must be positive", nameof(tagId));
            if (_articleTags.Exists(at => at.TagId == tagId)) return;
            _articleTags.Add(new ArticleTag(Id, tagId));
        }

        public void RemoveTag(int tagId)
        {
            _articleTags.RemoveAll(at => at.TagId == tagId);
        }

        public void MarkDeleted(DateTime nowUtc)
        {
            IsDeleted = true;
            DeletedAt = nowUtc;
            UpdatedAt = nowUtc;
        }
    }
}
