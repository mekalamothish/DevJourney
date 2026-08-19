using System.Collections.Generic;
using DevJourney.Application.ContentBlocks;

namespace DevJourney.Application.Dto.Articles
{
    public class ArticlePatchDto
    {
        // Value properties
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? Excerpt { get; set; }
        public string? FeaturedImage { get; set; }
        public int? ReadingTime { get; set; }
        public string? Status { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
        public List<ArticleBlock>? Content { get; set; }

        // Presence flags - controller must set these to indicate which properties were supplied
        public bool TitleProvided { get; set; }
        public bool SlugProvided { get; set; }
        public bool ExcerptProvided { get; set; }
        public bool FeaturedImageProvided { get; set; }
        public bool ReadingTimeProvided { get; set; }
        public bool StatusProvided { get; set; }
        public bool PublishedAtProvided { get; set; }
        public bool AuthorIdProvided { get; set; }
        public bool CategoryIdProvided { get; set; }
        public bool TagIdsProvided { get; set; }
        public bool ContentProvided { get; set; }
    }
}
