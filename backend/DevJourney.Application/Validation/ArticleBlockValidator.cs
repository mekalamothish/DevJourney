using DevJourney.Application.ContentBlocks;
using DevJourney.Application.Exceptions;

namespace DevJourney.Application.Validation
{
    public static class ArticleBlockValidator
    {
        public static void ValidateBlocks(IEnumerable<ArticleBlock> blocks)
        {
            if (blocks == null)
                throw new ValidationException("Content", "Content cannot be null");

            foreach (var block in blocks)
            {
                switch (block)
                {
                    case ParagraphBlock p:
                        if (string.IsNullOrWhiteSpace(p.Text))
                            throw new ValidationException("paragraph.text", "Paragraph text is required");
                        break;
                    case HeadingBlock h:
                        if (string.IsNullOrWhiteSpace(h.Text))
                            throw new ValidationException("heading.text", "Heading text is required");
                        if (h.Level != 2 && h.Level != 3)
                            throw new ValidationException("heading.level", "Heading level must be 2 or 3");
                        break;
                    case SubheadingBlock s:
                        if (string.IsNullOrWhiteSpace(s.Text))
                            throw new ValidationException("subheading.text", "Subheading text is required");
                        break;
                    case ListBlock l:
                        if (l.Items == null || !l.Items.Any())
                            throw new ValidationException("list.items", "List must have at least one item");
                        if (l.Items.Any(i => string.IsNullOrWhiteSpace(i)))
                            throw new ValidationException("list.items", "List items must not be empty");
                        break;
                    case CodeBlock c:
                        if (string.IsNullOrWhiteSpace(c.Code))
                            throw new ValidationException("code.code", "Code is required");
                        break;
                    case TerminalBlock t:
                        if (t.Lines == null || !t.Lines.Any())
                            throw new ValidationException("terminal.lines", "Terminal must include at least one line");
                        if (t.Lines.Any(l => string.IsNullOrWhiteSpace(l)))
                            throw new ValidationException("terminal.lines", "Terminal lines must not be empty");
                        break;
                    case QuoteBlock q:
                        if (string.IsNullOrWhiteSpace(q.Text))
                            throw new ValidationException("quote.text", "Quote text is required");
                        break;
                    case CalloutBlock co:
                        if (string.IsNullOrWhiteSpace(co.Text))
                            throw new ValidationException("callout.text", "Callout text is required");
                        break;
                    case TableBlock ta:
                        if (ta.Headers == null || !ta.Headers.Any())
                            throw new ValidationException("table.headers", "Table must have headers");
                        if (ta.Rows == null)
                            throw new ValidationException("table.rows", "Table rows required");
                        break;
                    case ImageBlock im:
                        if (string.IsNullOrWhiteSpace(im.Src))
                            throw new ValidationException("image.src", "Image src is required");
                        if (string.IsNullOrWhiteSpace(im.Alt))
                            throw new ValidationException("image.alt", "Image alt is required");
                        break;
                    case RichTextBlock r:
                        if (string.IsNullOrWhiteSpace(r.Html))
                            throw new ValidationException("richtext.html", "Rich text HTML is required");
                        break;
                    case TakeawaysBlock tk:
                        if (tk.Items == null || !tk.Items.Any())
                            throw new ValidationException("takeaways.items", "Takeaways must have at least one item");
                        break;
                    case FaqBlock fq:
                        if (fq.Items == null || !fq.Items.Any())
                            throw new ValidationException("faq.items", "FAQ must have at least one item");
                        foreach (var item in fq.Items)
                        {
                            if (string.IsNullOrWhiteSpace(item.Question) || string.IsNullOrWhiteSpace(item.Answer))
                                throw new ValidationException("faq.item", "FAQ items require both question and answer");
                        }
                        break;
                    default:
                        throw new ValidationException("content", $"Unsupported block type: {block?.GetType().Name}");
                }
            }
        }
    }
}
