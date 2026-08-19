using System;

namespace DevJourney.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing the article's structured content as JSON.
    /// The domain stores content as a canonical JSON string. Validation of the
    /// block shapes is handled at the application layer.
    /// </summary>
    public sealed class ArticleContent
    {
        public string Json { get; }

        public ArticleContent(string json)
        {
            Json = json ?? throw new ArgumentNullException(nameof(json));
        }

        public bool IsEmpty() => string.IsNullOrWhiteSpace(Json);

        public override string ToString() => Json;
    }
}
