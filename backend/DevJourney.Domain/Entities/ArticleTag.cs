namespace DevJourney.Domain.Entities
{
    public class ArticleTag
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }

        public ArticleTag(int articleId, int tagId)
        {
            ArticleId = articleId;
            TagId = tagId;
        }
    }
}
