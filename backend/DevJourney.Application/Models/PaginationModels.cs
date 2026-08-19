namespace DevJourney.Application.Models
{
    /// <summary>
    /// Pagination metadata returned with list responses.
    /// </summary>
    public class PaginationMetadata
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// Generic paged result wrapper for list responses.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public PaginationMetadata Meta { get; set; } = new();
    }
}
