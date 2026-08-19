API contract: DevJourney Frontend ↔ ASP.NET Core 10 Web API

Purpose
-------
This document is the FINAL V1 API contract for the DevJourney Angular frontend. It describes the backend resources, DTOs, endpoints, validation rules, and behaviors required by the frontend. It is design-only: no backend code is included here.

Conventions
-----------
- All endpoints return JSON.
- List responses use: { data: [...], meta: { total, page, pageSize } }
- Single resource responses use: { data: { ... } }
- Errors use: { error: { code, message, details? } }
- Canonical datetime format: ISO 8601 in UTC (e.g., 2026-08-18T11:00:00Z). Responses MUST use this format.
- If the client sends date-only strings (e.g., 2026-08-18) the server MAY accept them for compatibility but MUST normalize and store/send full ISO 8601 UTC datetimes in responses.
- IDs: integer primary keys (id: number).
- Pagination: page (1-based), pageSize (default 20, max 100).

Primary entities (overview)
---------------------------
V1 REQUIRED:
- Article (BlogPost)
- Category
- Tag
- Author
- ArticleBlock (structured content stored as JSON)

V1 RECOMMENDED:
- Resource / Media (for uploads and canonical URLs)

FUTURE / OPTIONAL (NOT V1 required):
- Preview API (frontend currently previews client-side)
- Advanced analytics, comments, reactions
- RoadmapItem / InterviewTopic (UI-only data)

Important: V1 focuses on the functionality the Angular app already implements. Do not add mandatory endpoints/features beyond that scope.

Article (BlogPost) — canonical fields
------------------------------------
This DTO represents what the frontend expects to receive. Request DTOs (create/update) use IDs for relations; responses expand related objects.

Response ArticleDTO (returned to frontend)
| Field | Type | Required | Notes |
|---|---|---|---|
| id | number | Yes | server-assigned
| title | string | Yes | editable
| slug | string | Yes | unique URL segment
| excerpt | string | Yes | editable
| featuredImage | string | No | canonical form: public URL (see Media section)
| readingTime | number | No | server-derived convenience (minutes)
| status | 'draft'|'published'|'archived' | Yes | lifecycle
| createdAt | string (ISO 8601 UTC) | Yes | server-set
| updatedAt | string (ISO 8601 UTC) | Yes | server-set
| publishedAt | string (ISO 8601 UTC) | No | set on publish
| author | { id,name,avatar?,role? } | Yes | expanded related object
| category | { id,name,slug } | Yes | expanded related object
| tags | array of { id,name,slug } | No | expanded related objects
| content | ArticleBlock[] | Yes | structured body
| isFeatured | boolean | No |
| isPopular | boolean | No |

Create / Update DTOs (requests)
- ArticleCreateDTO (POST /api/v1/articles)
{
  title: string,
  slug?: string, // server may generate if missing
  excerpt: string,
  featuredImage?: string, // canonical V1: public URL (see below)
  readingTime?: number, // server may override
  status?: 'draft'|'published'|'archived',
  publishedAt?: string (ISO or date-only accepted),
  authorId: number,
  categoryId: number,
  tagIds?: number[],
  content: ArticleBlock[]
}

- ArticleUpdateDTO (PUT or PATCH): same fields as create; PATCH may accept a partial set of fields.

Important rule: Requests MUST reference related entities by ID (authorId, categoryId, tagIds). The server will return expanded related objects in responses. Nested author/category/tag objects MUST NOT be used as the official request format in V1.

ArticleBlock[] — structured content (all block types used by the frontend)
-------------------------------------------------------------------------
The backend MUST accept and return the following block types. Store the array as JSON and validate per-type fields.

1) ParagraphBlock
{ "type": "paragraph", "text": "..." }

2) HeadingBlock
{ "type": "heading", "level": 2, "id": "section-1", "text": "Heading text" }
- level must be 2 or 3 (frontend uses H2/H3)

3) SubheadingBlock
{ "type": "subheading", "id": "sub-1", "text": "..." }

4) ListBlock
{ "type": "list", "ordered": false, "items": ["one","two"] }

5) CodeBlock
{ "type": "code", "language": "typescript", "code": "...", "filename"?: "file.ts" }

6) TerminalBlock (canonical V1)
{ "type": "terminal", "lines": ["npm install", "ng serve"] }
- lines: string[] is REQUIRED for terminal blocks in V1.
- Frontend renderer should join lines with '\n' when rendering.
- Do NOT include a `commands` property in the V1 contract.

7) QuoteBlock
{ "type": "quote", "text": "...", "author"?: "Name" }

8) CalloutBlock
{ "type": "callout", "variant": "note" | "tip" | "warning" | "important", "heading"?: "...", "text": "..." }

9) TableBlock
{ "type": "table", "headers": ["A","B"], "rows": [["1","2"],["3","4"]], "caption"?: "..." }

10) ImageBlock
{ "type": "image", "src": "https://cdn.example.com/...", "alt": "...", "caption"?: "..." }
- See Media section for canonical V1 semantics of src.

11) TakeawaysBlock
{ "type": "takeaways", "items": ["...","..."] }

12) FaqBlock
{ "type": "faq", "items": [{ "q": "...", "a": "..." }] }

Backend responsibilities for ArticleBlock[]
- Validate block.type and required fields per type.
- Validate heading.level in allowed values.
- Enforce total content size limits (e.g., max JSON size) and per-block length limits as appropriate.
- Preserve block ordering; do not transform the shape for editor convenience.

Author
------
Representation (response):
{ id: number, name: string, avatar?: string, role?: string, bio?: string }
- Backend exposes authors via API. Requests reference author by authorId.

Category and Tag
----------------
Representation (response):
Category: { id:number, name:string, slug:string }
Tag: { id:number, name:string, slug:string }
- Requests reference by categoryId and tagIds.
- List endpoints may optionally include articleCount when requested.

Media / ImageBlock (V1 canonical)
---------------------------------
- Canonical V1 form for ImageBlock.src and featuredImage is a public URL (string). Example: "https://cdn.example.com/posts/1/hero.jpg".
- A media upload endpoint is RECOMMENDED but NOT REQUIRED for V1. If implemented, it should return a public URL which the frontend will use as the canonical src.
- The backend may support resource IDs internally, but for V1 the contract exposes and expects URLs in Article requests/responses. Do NOT require clients to send resource IDs for V1.
- Validate that ImageBlock.src is a well-formed URL and, if strict validation is enabled, that the resource exists or was uploaded via the media endpoint.

API Endpoints (V1) — minimal required surface
--------------------------------------------
Base path: /api/v1

Articles (REQUIRED)
GET /api/v1/articles
- Query params: page, pageSize, q, category (slug or id), tag (slug or id), authorId, status, sort, since, until
- Returns: { data: ArticleDTO[], meta: { total, page, pageSize } }
- Default: status=published (public API)

GET /api/v1/articles/{id}
- Returns ArticleDTO (404 if not found or not published for public calls)

GET /api/v1/articles/slug/{slug}
- Returns ArticleDTO by slug

POST /api/v1/articles
- Create article (admin only)
- Body: ArticleCreateDTO (use IDs for relations)
- Returns 201 + { data: ArticleDTO }

PUT /api/v1/articles/{id}
- Replace/update (admin only) — full replacement
- Body: ArticleCreateDTO
- Returns 200 + { data: ArticleDTO }

PATCH /api/v1/articles/{id}
- Partial update (admin only)

DELETE /api/v1/articles/{id}
- Soft or hard delete per server policy — return 204

POST /api/v1/articles/{id}/publish
- Sets status=published and publishedAt (server sets if not provided) — returns updated Article

POST /api/v1/articles/{id}/unpublish
- Sets status=draft and MAY clear publishedAt

Categories (REQUIRED)
GET /api/v1/categories
POST /api/v1/categories
PUT /api/v1/categories/{id}
DELETE /api/v1/categories/{id}
- includeCounts=true optional

Tags (REQUIRED)
GET /api/v1/tags
POST /api/v1/tags
PUT /api/v1/tags/{id}
DELETE /api/v1/tags/{id}

Authors (REQUIRED)
GET /api/v1/authors
GET /api/v1/authors/{id}
POST /api/v1/authors
PUT /api/v1/authors/{id}

Media (RECOMMENDED)
POST /api/v1/media (multipart/form-data) -> returns public URL
GET /api/v1/media/{id}
DELETE /api/v1/media/{id}
- Media endpoints are RECOMMENDED for upload workflows; however, V1 clients can supply public URLs directly in requests.

Optional / Future (NOT V1 required)
- GET /api/v1/articles/{id}/related (recommended for related posts)
- GET /api/v1/articles/{id}/prevnext (optional convenience)
- Preview endpoint: NOT required — frontend currently previews client-side.

Search & pagination
-------------------
- q searches title, excerpt, and optionally content text.
- Support filtering by category, tag, author, status.
- Responses include meta.total; support total=false to skip expensive counts.

DTOs (shapes) — final
---------------------
ArticleDTO (response)
{
  id: number,
  title: string,
  slug: string,
  excerpt: string,
  featuredImage?: string, // public URL
  readingTime?: number,
  status: 'draft'|'published'|'archived',
  createdAt: string, // ISO 8601 UTC
  updatedAt: string, // ISO 8601 UTC
  publishedAt?: string, // ISO 8601 UTC
  author: { id:number, name:string, avatar?:string, role?:string },
  category: { id:number, name:string, slug:string },
  tags: { id:number, name:string, slug:string }[],
  content: ArticleBlock[],
  isFeatured?: boolean,
  isPopular?: boolean
}

ArticleCreateDTO (request)
{
  title: string,
  slug?: string,
  excerpt: string,
  featuredImage?: string, // public URL
  readingTime?: number,
  status?: 'draft'|'published'|'archived',
  publishedAt?: string,
  authorId: number,
  categoryId: number,
  tagIds?: number[],
  content: ArticleBlock[]
}

ArticleUpdateDTO
- Same as create DTO. PATCH may accept partial fields.

Validation rules (V1)
---------------------
- title: required, max 255 chars
- slug: required (or server-generated), url-safe, unique, max 255
- excerpt: required, max 1000 chars
- content: required, non-empty array, JSON size limits
- categoryId: required and must exist
- authorId: required and must exist
- tagIds: optional
- ImageBlock.src: should be a public URL (validate format)
- Dates provided may be date-only or full ISO; backend stores and returns full ISO 8601 UTC

Status & publish semantics
--------------------------
- status values: 'draft', 'published', 'archived'
- Publishing sets status='published' and publishedAt timestamp (server sets if not present)
- Unpublish sets status='draft' and MAY clear publishedAt
- Archived items are excluded from public lists

Slug behavior
-------------
- Ensure slug uniqueness; return 409 Conflict on duplicates.
- Server may auto-generate slugs from title with conflict resolution (-1,-2)

Errors
------
Error response shape
{
  "error": { "code": "...", "message": "...", "details"?: {} }
}

Important HTTP statuses to use
- 200 OK (successful GET/PUT/PATCH)
- 201 Created (POST)
- 204 No Content (DELETE)
- 400 Bad Request (malformed input)
- 401 Unauthorized (auth required)
- 403 Forbidden (insufficient permissions)
- 404 Not Found
- 409 Conflict (e.g., slug conflict)
- 422 Unprocessable Entity (validation errors)

Operational notes
-----------------
- Soft delete is recommended but optional for V1.
- Index slug, publishedAt, status, category for performance.
- Full-text search is optional; start simple and iterate.

Implementation checklist (V1 priorities)
---------------------------------------
V1 REQUIRED
- Implement Article endpoints (CRUD, publish/unpublish)
- Implement Category and Tag endpoints (CRUD)
- Implement Author endpoints (CRUD)
- Validate and store ArticleBlock[] JSON as-is (including subheading and terminal.lines)
- Enforce slug uniqueness and publishing rules
- Support pagination, search (basic q), and category/tag filtering

V1 RECOMMENDED
- Media upload endpoint returning public URLs
- Related/prev-next convenience endpoints

FUTURE / OPTIONAL
- Admin preview API
- Advanced search with relevance scoring
- Analytics, comments, reactions, content versioning

End of contract — V1
-------------------
This file is the FINAL V1 API contract for the Angular frontend. It has been aligned with the existing codebase and the validated findings (terminal.lines canonical, request DTOs using IDs, ISO 8601 UTC dates, client-side preview, ImageBlock src canonical as public URL, subheading block included, readingTime server-derived).

If frontend models change, update this contract before backend implementation.