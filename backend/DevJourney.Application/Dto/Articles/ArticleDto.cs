namespace DevJourney.Application.Dto.Articles
{
    using DevJourney.Application.ContentBlocks;
    using DevJourney.Application.Dto.Common;

    /// <summary>
    /// Response DTO for an Article (BlogPost).
    /// </summary>
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public string? FeaturedImage { get; set; }
        public int? ReadingTime { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        
        public AuthorDto Author { get; set; }
        public CategoryDto Category { get; set; }
        public List<TagDto> Tags { get; set; } = new();
        
        public List<ArticleBlock> Content { get; set; } = new();
        public bool IsFeatured { get; set; }
        public bool IsPopular { get; set; }
    }
}
