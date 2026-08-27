namespace DevJourney.Application.ContentBlocks
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Base class for all article block types.
    /// Polymorphic representation using type discriminator.
    /// </summary>
    [JsonConverter(typeof(ArticleBlockConverter))]
    public abstract class ArticleBlock
    {
        [JsonIgnore]
        public abstract string Type { get; }
    }

    /// <summary>
    /// Paragraph block: plain text content.
    /// </summary>
    public class ParagraphBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "paragraph";
        public string Text { get; set; }
    }

    /// <summary>
    /// Heading block: H2 or H3 heading with optional ID.
    /// </summary>
    public class HeadingBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "heading";
        public int Level { get; set; }
        public string? Id { get; set; }
        public string Text { get; set; }
    }

    /// <summary>
    /// Subheading block: alternative heading style.
    /// </summary>
    public class SubheadingBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "subheading";
        public string? Id { get; set; }
        public string Text { get; set; }
    }

    /// <summary>
    /// List block: ordered or unordered list.
    /// </summary>
    public class ListBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "list";
        public bool Ordered { get; set; }
        public List<string> Items { get; set; } = new();
    }

    /// <summary>
    /// Code block: code snippet with language and optional filename.
    /// </summary>
    public class CodeBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "code";
        public string Language { get; set; }
        public string Code { get; set; }
        public string? Filename { get; set; }
    }

    /// <summary>
    /// Terminal block: terminal command lines.
    /// Uses lines: string[] (canonical V1 format).
    /// </summary>
    public class TerminalBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "terminal";
        public List<string> Lines { get; set; } = new();
    }

    /// <summary>
    /// Quote block: blockquote with optional author.
    /// </summary>
    public class QuoteBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "quote";
        public string Text { get; set; }
        public string? Author { get; set; }
    }

    /// <summary>
    /// Callout block: note/tip/warning/important alert.
    /// </summary>
    public class CalloutBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "callout";
        public string Variant { get; set; }
        public string? Heading { get; set; }
        public string Text { get; set; }
    }

    /// <summary>
    /// Table block: tabular data with headers and rows.
    /// </summary>
    public class TableBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "table";
        public List<string> Headers { get; set; } = new();
        public List<List<string>> Rows { get; set; } = new();
        public string? Caption { get; set; }
    }

    /// <summary>
    /// Image block: embedded image with alt text and optional caption.
    /// </summary>
    public class ImageBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "image";
        public string Src { get; set; }
        public string Alt { get; set; }
        public string? Caption { get; set; }
    }

    /// <summary>
    /// Rich text block: arbitrary HTML produced by a rich editor.
    /// </summary>
    public class RichTextBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "richtext";
        public string Html { get; set; }
    }

    /// <summary>
    /// Takeaways block: list of key takeaways.
    /// </summary>
    public class TakeawaysBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "takeaways";
        public List<string> Items { get; set; } = new();
    }

    /// <summary>
    /// FAQ block: list of Q&A items.
    /// </summary>
    public class FaqBlock : ArticleBlock
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public override string Type => "faq";
        public List<FaqItem> Items { get; set; } = new();
    }

    /// <summary>
    /// FAQ item: question and answer pair.
    /// </summary>
    public class FaqItem
    {
        [JsonPropertyName("q")]
        public string Question { get; set; }
        
        [JsonPropertyName("a")]
        public string Answer { get; set; }
    }
}
