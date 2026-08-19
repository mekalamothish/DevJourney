using Microsoft.AspNetCore.Mvc;
using DevJourney.Application.Interfaces;
using DevJourney.Application.Models;
using DevJourney.Application.Dto.Articles;

namespace DevJourney.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _service;

        public ArticlesController(IArticleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ArticleQuery query)
        {
            // Public API default: return published articles when status not explicitly provided
            if (string.IsNullOrWhiteSpace(query.Status))
            {
                query.Status = "published";
            }

            var result = await _service.GetPagedAsync(query);
            return Ok(new { data = result.Data, meta = result.Meta });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return Ok(new { data = dto });
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var dto = await _service.GetBySlugAsync(slug);
            return Ok(new { data = dto });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticleCreateDto create)
        {
            var created = await _service.CreateAsync(create);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { data = created });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ArticleUpdateDto update)
        {
            var updated = await _service.UpdateAsync(id, update);
            return Ok(new { data = updated });
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, [FromBody] System.Text.Json.JsonElement body)
        {
            // Detect which fields are present and build an ArticlePatchDto for the application
            var patch = new DevJourney.Application.Dto.Articles.ArticlePatchDto();

            if (body.ValueKind != System.Text.Json.JsonValueKind.Object)
            {
                // empty or invalid body
                throw new DevJourney.Application.Exceptions.ValidationException("", "Invalid PATCH payload");
            }

            if (body.TryGetProperty("title", out var titleProp))
            {
                patch.TitleProvided = true;
                patch.Title = titleProp.ValueKind == System.Text.Json.JsonValueKind.Null ? null : titleProp.GetString();
            }

            if (body.TryGetProperty("slug", out var slugProp))
            {
                patch.SlugProvided = true;
                patch.Slug = slugProp.ValueKind == System.Text.Json.JsonValueKind.Null ? null : slugProp.GetString();
            }

            if (body.TryGetProperty("excerpt", out var excerptProp))
            {
                patch.ExcerptProvided = true;
                patch.Excerpt = excerptProp.ValueKind == System.Text.Json.JsonValueKind.Null ? null : excerptProp.GetString();
            }

            if (body.TryGetProperty("featuredImage", out var fiProp))
            {
                patch.FeaturedImageProvided = true;
                patch.FeaturedImage = fiProp.ValueKind == System.Text.Json.JsonValueKind.Null ? null : fiProp.GetString();
            }

            if (body.TryGetProperty("readingTime", out var rtProp))
            {
                patch.ReadingTimeProvided = true;
                if (rtProp.ValueKind == System.Text.Json.JsonValueKind.Null) patch.ReadingTime = null;
                else if (rtProp.TryGetInt32(out var rt)) patch.ReadingTime = rt;
            }

            if (body.TryGetProperty("status", out var statusProp))
            {
                patch.StatusProvided = true;
                patch.Status = statusProp.ValueKind == System.Text.Json.JsonValueKind.Null ? null : statusProp.GetString();
            }

            if (body.TryGetProperty("publishedAt", out var pAtProp))
            {
                patch.PublishedAtProvided = true;
                if (pAtProp.ValueKind == System.Text.Json.JsonValueKind.Null) patch.PublishedAt = null;
                else if (pAtProp.ValueKind == System.Text.Json.JsonValueKind.String && DateTime.TryParse(pAtProp.GetString(), out var dt)) patch.PublishedAt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }

            if (body.TryGetProperty("authorId", out var aProp))
            {
                patch.AuthorIdProvided = true;
                if (aProp.ValueKind == System.Text.Json.JsonValueKind.Null) patch.AuthorId = null;
                else if (aProp.TryGetInt32(out var aid)) patch.AuthorId = aid;
            }

            if (body.TryGetProperty("categoryId", out var cProp))
            {
                patch.CategoryIdProvided = true;
                if (cProp.ValueKind == System.Text.Json.JsonValueKind.Null) patch.CategoryId = null;
                else if (cProp.TryGetInt32(out var cid)) patch.CategoryId = cid;
            }

            if (body.TryGetProperty("tagIds", out var tProp))
            {
                patch.TagIdsProvided = true;
                if (tProp.ValueKind == System.Text.Json.JsonValueKind.Null) patch.TagIds = null;
                else if (tProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    var list = new System.Collections.Generic.List<int>();
                    foreach (var je in tProp.EnumerateArray())
                    {
                        if (je.ValueKind == System.Text.Json.JsonValueKind.Number && je.TryGetInt32(out var idv)) list.Add(idv);
                    }
                    patch.TagIds = list;
                }
            }

            if (body.TryGetProperty("content", out var contentProp))
            {
                patch.ContentProvided = true;
                if (contentProp.ValueKind == System.Text.Json.JsonValueKind.Null) patch.Content = null;
                else
                {
                    try
                    {
                        var options = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                        };
                        options.Converters.Add(new DevJourney.Application.ContentBlocks.ArticleBlockConverter());
                        patch.Content = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<DevJourney.Application.ContentBlocks.ArticleBlock>>(contentProp.GetRawText(), options)
                            ?? new System.Collections.Generic.List<DevJourney.Application.ContentBlocks.ArticleBlock>();
                    }
                    catch (System.Text.Json.JsonException ex)
                    {
                        throw new DevJourney.Application.Exceptions.ValidationException("content", "Invalid content payload: " + ex.Message);
                    }
                }
            }

            var updated = await _service.PatchAsync(id, patch);
            return Ok(new { data = updated });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            var updated = await _service.PublishAsync(id);
            return Ok(new { data = updated });
        }

        [HttpPost("{id:int}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            var updated = await _service.UnpublishAsync(id);
            return Ok(new { data = updated });
        }
    }
}
