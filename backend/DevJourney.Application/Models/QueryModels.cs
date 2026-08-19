namespace DevJourney.Application.Models
{
    /// <summary>
    /// Query/filter model for listing articles.
    /// Supports the filters defined in the API contract.
    /// </summary>
    public class ArticleQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Q { get; set; }
        public string? Category { get; set; }
        public string? Tag { get; set; }
        public int? AuthorId { get; set; }
        public string? Status { get; set; }
        public string? Sort { get; set; }
        public DateTime? Since { get; set; }
        public DateTime? Until { get; set; }
    }

    /// <summary>
    /// Query/filter model for listing categories.
    /// </summary>
    public class CategoryQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool IncludeCounts { get; set; }
    }

    /// <summary>
    /// Query/filter model for listing tags.
    /// </summary>
    public class TagQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Q { get; set; }
    }

    /// <summary>
    /// Query/filter model for listing authors.
    /// </summary>
    public class AuthorQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
