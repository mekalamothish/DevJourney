using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevJourney.Application.ContentBlocks
{
    public class ArticleBlockConverter : JsonConverter<ArticleBlock>
    {
        public override ArticleBlock? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected start object for ArticleBlock");

            using var doc = JsonDocument.ParseValue(ref reader);
            if (!doc.RootElement.TryGetProperty("type", out var typeProp))
                throw new JsonException("ArticleBlock missing 'type' discriminator");

            var type = typeProp.GetString();
            var json = doc.RootElement.GetRawText();

            return type switch
            {
                "paragraph" => JsonSerializer.Deserialize<ParagraphBlock>(json, options),
                "heading" => JsonSerializer.Deserialize<HeadingBlock>(json, options),
                "subheading" => JsonSerializer.Deserialize<SubheadingBlock>(json, options),
                "list" => JsonSerializer.Deserialize<ListBlock>(json, options),
                "code" => JsonSerializer.Deserialize<CodeBlock>(json, options),
                "terminal" => JsonSerializer.Deserialize<TerminalBlock>(json, options),
                "quote" => JsonSerializer.Deserialize<QuoteBlock>(json, options),
                "callout" => JsonSerializer.Deserialize<CalloutBlock>(json, options),
                "table" => JsonSerializer.Deserialize<TableBlock>(json, options),
                "image" => JsonSerializer.Deserialize<ImageBlock>(json, options),
                // Legacy richtext blocks: deserialize then convert to paragraph to preserve
                // API contract compatibility (richtext is a UI-only block in frontend).
                "richtext" => ConvertRichTextToParagraph(JsonSerializer.Deserialize<RichTextBlock>(json, options)),
                "takeaways" => JsonSerializer.Deserialize<TakeawaysBlock>(json, options),
                "faq" => JsonSerializer.Deserialize<FaqBlock>(json, options),
                _ => throw new JsonException($"Unknown ArticleBlock type: {type}")
            };
        }

        public override void Write(Utf8JsonWriter writer, ArticleBlock value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // When writing, ensure we never emit a persisted 'richtext' discriminator.
            // If a RichTextBlock is present (from legacy data), serialize it as a paragraph.
            if (value is RichTextBlock rich)
            {
                var paragraph = new ParagraphBlock { Text = rich.Html };
                value = paragraph;
            }

            using var doc = JsonDocument.Parse(JsonSerializer.Serialize(value, value.GetType(), options));
            writer.WriteStartObject();
            writer.WriteString("type", value.Type);

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                // skip any existing 'type' property from serialized object
                if (string.Equals(prop.Name, "type", StringComparison.OrdinalIgnoreCase)) continue;
                prop.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        private static ArticleBlock? ConvertRichTextToParagraph(RichTextBlock? rich)
        {
            if (rich == null) return null;
            return new ParagraphBlock { Text = rich.Html };
        }
    }
}
