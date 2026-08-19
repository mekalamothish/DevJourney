namespace DevJourney.Application.Dto.Articles
{
    using DevJourney.Application.ContentBlocks;

    /// <summary>
    /// Request DTO for creating an Article.
    /// </summary>
    public class ArticleCreateDto
    {
        public string Title { get; set; }
        public string? Slug { get; set; }
        public string Excerpt { get; set; }
        public string? FeaturedImage { get; set; }
        public int? ReadingTime { get; set; }
        public string? Status { get; set; }
        public DateTime? PublishedAt { get; set; }
        
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
        
        public List<ArticleBlock> Content { get; set; } = new();
    }
}
