# Step 9 — Frontend Review and API Contract Design

The Angular frontend is now functionally implemented with:

- Angular 21
- Tailwind CSS
- DevJourney design system
- Public layout
- Admin layout
- Home page
- Blog listing
- Blog detail page
- Article preview
- Admin article management
- Article editor
- Categories management
- Tags management
- Structured `ArticleBlock[]` content
- Mock/in-memory `BlogDataService`

**DO NOT implement any backend code yet.**

This step is an analysis and contract-design step.

The goal is to inspect the entire Angular application and determine exactly what the future ASP.NET Core 10 backend must provide.

---

## 1. IMPORTANT — STOP CODING UI

Do NOT:

- create .NET projects
- create controllers
- create EF Core entities
- create DbContext
- create migrations
- create SQL tables
- add HttpClient
- connect Angular to an API
- add authentication
- add authorization

Do not change the existing UI unless a small model/interface change is required to make the API contract clear.

The output of this step should primarily be documentation.

---

## 2. Inspect the Entire Angular Project

Review:

```
src/app/
including:
  core/
  shared/
  layout/
  features/home/
  features/blog/
  features/admin/
```

Inspect:

- models
- services
- components
- pages
- routes
- mock data
- forms
- search
- filtering
- pagination
- article editor
- preview
- categories
- tags
- resources
- roadmap
- interview preparation

Do not rely only on previous implementation summaries. Inspect the actual source code.

---

## 3. Create API Requirements Document

Create:

```
docs/api-contract.md
```

This document will become the contract between:

```
Angular 21
   ↓
ASP.NET Core 10 Web API
```

The document must be detailed enough that the backend can later be implemented without guessing the frontend requirements.

---

## 4. Identify All Backend Resources

Determine which actual backend resources/entities are required.

At minimum investigate:

- Article
- Category
- Tag
- Author
- ArticleContent / ArticleBlock
- Resource
- RoadmapItem
- InterviewTopic

Do NOT automatically create a database entity for every frontend component. For example:

- Hero
- FeaturedArticle
- LatestArticles
- PopularArticles
- Topics
- FinalCta

are UI concepts and may not require database tables.

Explain which concepts are:

- persistent entities
- derived data
- static configuration
- UI-only concepts

For every candidate entity explain whether it is required for V1 or should remain future scope.

---

## 5. Article Requirements

Analyze everything the Angular application currently needs for a BlogPost.

Document fields such as:

- id
- title
- slug
- excerpt
- featuredImage
- readingTime
- status
- createdAt
- updatedAt
- publishedAt
- author
- category
- tags
- content

For every field document:

| Field | Type | Required | Editable | Persisted? | Derived? | Public? | Purpose |
|---|---|---|---|---|---|---|---|

Also identify:

- whether the field belongs in the database
- whether it is derived
- whether it is editable by admin
- whether it is returned publicly
- whether it is required for SEO
- whether it is required only by the admin editor

Do not invent fields unless the application genuinely needs them.

---

## 6. Article Status

Document the API representation of:

- draft
- published
- archived

Determine:

- how status is stored
- how publishing works
- how archiving works
- whether `publishedAt` is automatically assigned
- whether unpublishing clears `publishedAt`
- whether an archived article can be published again
- whether draft articles are visible publicly
- whether archived articles are visible publicly

Recommend a clean approach. The recommendation should be suitable for a future authenticated admin system.

---

## 7. Article Content

This is one of the most important parts of the analysis.

The current Angular application uses:

```
ArticleBlock[]
```

Inspect the actual `ArticleBlock` model in the project. Document every supported block.

At minimum investigate:

- paragraph
- heading
- list
- code
- terminal
- quote
- callout
- table
- image
- takeaways
- faq

For every block document its exact structure. For example:

```json
{
  "type": "code",
  "language": "typescript",
  "filename": "counter.ts",
  "code": "const count = signal(0);"
}
```

Another example:

```json
{
  "type": "heading",
  "level": 2,
  "id": "understanding-signals",
  "text": "Understanding Signals"
}
```

Determine how this should be represented by the backend.

Do NOT prematurely split every content block into a separate SQL table. Evaluate whether article content should initially be stored as JSON inside the Article record.

Analyze:

- **Option A** — Store the complete article content as JSON.
- **Option B** — Create separate tables for every block type.
- **Option C** — Use another approach.

For this project recommend the simplest maintainable approach. Explain:

- advantages
- disadvantages
- querying implications
- versioning implications
- editing implications
- EF Core implications
- SQL Server implications

Do not implement it yet.

---

## 8. Article API Endpoints

Design the required article endpoints based on the actual Angular application.

At minimum investigate:

```
GET    /api/articles
GET    /api/articles/{id}
GET    /api/articles/slug/{slug}

POST   /api/articles
PUT    /api/articles/{id}
DELETE /api/articles/{id}

POST   /api/articles/{id}/publish
POST   /api/articles/{id}/unpublish
POST   /api/articles/{id}/archive
```

But do NOT blindly use this list. Derive the actual endpoints from the Angular application.

For every endpoint document:

- HTTP Method
- URL
- Purpose
- Authentication requirement
- Query parameters
- Request body
- Response body
- Success status code
- Possible error responses

Authentication can be marked **Future** — because authentication is intentionally not implemented yet.

Also identify whether some operations should be combined rather than having unnecessary endpoints.

---

## 9. Article Listing API

Analyze the Angular blog listing page. It currently supports:

- search
- category filter
- pagination
- sorting/latest ordering

Design the backend API accordingly.

Example candidate:

```
GET /api/articles
```

with:

- search
- category
- page
- pageSize
- status
- sort

Determine the final query parameters. Example:

```
GET /api/articles?search=angular&category=angular&page=1&pageSize=9
```

Design a consistent response. Recommended structure:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 9,
  "totalItems": 42,
  "totalPages": 5
}
```

Determine whether additional metadata is required. Do not use different pagination structures for different endpoints unless necessary.

---

## 10. Public vs Admin Article APIs

Analyze the difference between public and admin requirements.

**Public** — Public users should normally receive: published articles only.

**Admin** — Admin users will eventually need: draft, published, archived.

Determine whether the API should expose:

- **Option A** — `/api/articles` and `/api/admin/articles`
- **Option B** — `/api/articles?status=...`
- or another approach

Recommend one approach. Consider:

- future authorization
- separation of public/admin behavior
- maintainability
- API clarity
- security

Remember: Authentication is NOT being implemented yet.

---

## 11. Featured / Latest / Popular

Inspect the Home page. Determine which Home sections require backend support.

Analyze:

- Featured Article
- Latest Articles
- Popular Articles
- Topics
- Learning Roadmap
- Interview Preparation
- Resources
- Author
- Final CTA

For each determine:

| Home Section | API Required? | Database Required? | Derived? | Static? | V1/Future |
|---|---|---|---|---|---|

For example:

- **Latest Articles** → derived from published articles ordered by `publishedAt`
- **Featured Article** → could be explicitly marked as featured
- **Popular Articles** → may eventually require view/read statistics
- **Topics** → can be derived from categories/tags

Do not create unnecessary tables.

---

## 12. Popular Articles

This needs special consideration. The frontend currently has mock popular articles. Determine what the future backend should do.

Possible approaches:

- **Option A** — Store `viewCount` directly on Article.
- **Option B** — Create `ArticleView` or another analytics entity.
- **Option C** — Use an external analytics system later.

Analyze the trade-offs. For the first backend version, recommend the simplest approach that works. Do not implement analytics now — just document the future requirement.

---

## 13. Category API

Design category endpoints based on the Angular admin UI. Likely:

```
GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

Determine whether `articleCount` should be returned. If yes, determine whether:

- it should be calculated dynamically
- stored separately
- returned only for admin

Also determine:

- category name uniqueness
- slug uniqueness
- deletion behavior if articles use the category
- whether an article must always have a category

Document all decisions.

---

## 14. Tag API

Design:

```
GET    /api/tags
GET    /api/tags/{id}
POST   /api/tags
PUT    /api/tags/{id}
DELETE /api/tags/{id}
```

Determine:

- many-to-many relationship with articles
- article count
- slug
- uniqueness rules
- deletion behavior
- whether tags are required
- whether an article can have zero tags
- whether duplicate tag assignments are allowed

Recommend a clean database/API representation.

---

## 15. Slug Requirements

Document slug behavior for:

- Article
- Category
- Tag

For example:

```
title → slug
Understanding Angular Signals
        ↓
understanding-angular-signals
```

Determine:

- uniqueness
- lowercase requirements
- allowed characters
- spaces
- special characters
- whether slug can be manually edited
- what happens when slug changes
- whether old slugs need redirects
- SEO implications

For articles, consider whether future slug history/redirect support may be needed. Do not implement redirects now unless required.

---

## 16. Author

The current application has an Author model. Determine whether V1 should have an Authors table, or whether author information can temporarily be static/configuration data.

Authentication/authorization is NOT being implemented yet.

Analyze:

- single-author blog
- multiple authors in future
- author profile
- avatar
- bio
- social links
- author/article relationship

Recommend the simplest approach for V1 while keeping future expansion possible.

---

## 17. Resources

Inspect the existing Resources section and implementation. Determine whether resources should become a persistent entity.

Possible fields:

- id
- title
- description
- url
- type
- icon
- displayOrder
- isActive
- createdAt
- updatedAt

Determine:

- whether Resources are currently static
- whether admin editing is required
- whether database persistence is needed
- whether an API is required

Mark the recommendation as: **V1** / **Future** / **Static**.

Do not create a resource table simply because a Resource UI component exists.

---

## 18. Roadmap

Inspect: Learning Roadmap. Determine whether roadmap items should be stored in the database.

Potential structure:

- id
- title
- description
- displayOrder
- status

Consider whether the roadmap needs:

- categories
- completion status
- ordering
- links
- descriptions
- dates

Determine whether it can reasonably remain static for V1. Give a recommendation. Do not create a database table unless there is a clear requirement.

---

## 19. Interview Preparation

Inspect the current Interview Preparation section. Determine whether `InterviewTopic` needs to be a database entity.

Consider possible future functionality:

- topic management
- interview questions
- explanations
- notes
- preparation status
- difficulty
- category
- links
- progress tracking

Clearly separate **Required now** from **Future**. Do not over-engineer V1.

---

## 20. API DTOs

Do NOT expose database entities directly from the API. Design DTOs.

At minimum consider:

- `ArticleListItemDto`
- `ArticleDetailDto`
- `CreateArticleRequest`
- `UpdateArticleRequest`
- `CategoryDto`
- `CreateCategoryRequest`
- `UpdateCategoryRequest`
- `TagDto`
- `CreateTagRequest`
- `UpdateTagRequest`

If additional DTOs are required, identify them. For every DTO document the JSON shape. Example:

```json
{
  "id": 1,
  "title": "Understanding Angular Signals",
  "slug": "understanding-angular-signals",
  "excerpt": "A practical guide...",
  "category": {
    "id": 1,
    "name": "Angular",
    "slug": "angular"
  },
  "tags": [],
  "readingTime": 8,
  "publishedAt": "2026-08-18T10:00:00Z"
}
```

Clearly distinguish:

- Request DTO
- Response DTO
- List DTO
- Detail DTO

---

## 21. Request / Response Examples

Every write endpoint should have an example request and response. For example:

**POST /api/articles**

Request:

```json
{
  "title": "Understanding Angular Signals",
  "slug": "understanding-angular-signals",
  "excerpt": "A practical guide...",
  "categoryId": 1,
  "tagIds": [1, 2],
  "featuredImage": null,
  "readingTime": 8,
  "status": "draft",
  "content": []
}
```

Response:

```json
{
  "id": 10,
  "title": "Understanding Angular Signals",
  "slug": "understanding-angular-signals",
  "excerpt": "A practical guide...",
  "category": {
    "id": 1,
    "name": "Angular",
    "slug": "angular"
  },
  "tags": [],
  "featuredImage": null,
  "readingTime": 8,
  "status": "draft",
  "createdAt": "2026-08-18T10:00:00Z",
  "updatedAt": "2026-08-18T10:00:00Z",
  "publishedAt": null,
  "content": []
}
```

Do this for the important create/update operations.

---

## 22. Error Contract

Design one consistent API error structure. For example:

```json
{
  "status": 400,
  "title": "Validation failed",
  "errors": {
    "title": ["Title is required."],
    "slug": ["Slug already exists."]
  }
}
```

Consider:

- 400
- 404
- 409
- 422
- 500

Determine which errors should be used. Recommend one consistent error contract for the entire API. Do not implement it yet.

---

## 23. HTTP Status Codes

Document the expected status code for each operation. Examples:

- GET successful → 200 OK
- POST successful → 201 Created
- PUT successful → 200 OK
- DELETE successful → 204 No Content
- Resource not found → 404 Not Found
- Validation failure → 400 Bad Request
- Duplicate/conflict → 409 Conflict
- Server error → 500 Internal Server Error

Use REST conventions consistently. Create a table showing:

| Operation | Success | Common Errors |
|---|---|---|

---

## 24. API Naming Conventions

Define:

- plural resource names
- route naming
- query parameter naming
- JSON casing
- date format
- enum representation
- ID representation

Recommended JSON naming: **camelCase**. Example:

```json
{
  "publishedAt": "2026-08-18T10:30:00Z"
}
```

Recommended API routes should be predictable. Avoid inconsistent patterns such as:

```
/api/GetArticles
/api/articleList
/api/getArticleBySlug
```

Prefer REST-style routes.

---

## 25. Date / Time

Determine how the backend should represent:

- createdAt
- updatedAt
- publishedAt

Recommend: **UTC**, **ISO 8601**. Example:

```
2026-08-18T10:30:00Z
```

Document:

- backend storage
- API serialization
- Angular display formatting
- whether clients should convert to local time

Do not use local server time.

---

## 26. Image Handling

The current frontend uses image URLs and placeholders. Do NOT implement image upload yet.

Document future possibilities:

- **Option A** — URL-based images.
- **Option B** — File upload.
- **Option C** — Cloud object storage.

Determine the recommended V1 approach. Also identify what the Article image block currently requires. Do not implement image storage yet.

---

## 27. Database Relationship Diagram

Create a conceptual relationship diagram in Markdown. At minimum evaluate:

```
Article
   |
   +---- Category
   |
   +---- Author
   |
   +---- ArticleTag ---- Tag
```

And:

```
Article
   |
   +---- Content JSON
```

If the recommended design differs, explain it. Do not create actual EF Core models yet.

---

## 28. Database Table Proposal

Create a proposed table list. For example:

**Required for V1**

- Articles
- Categories
- Tags
- ArticleTags

Then evaluate optional entities:

- Authors
- Resources
- RoadmapItems
- InterviewTopics
- ArticleViews

Clearly mark each as:

- Required for V1
- Optional
- Future
- Not required

For every V1 table identify:

- primary key
- important columns
- foreign keys
- unique constraints
- indexes

Do NOT create the actual database.

---

## 29. Search

Analyze the current frontend search. The current search checks:

- title
- excerpt
- category
- tags

Determine what the backend should search. Recommend a simple initial implementation. For V1:

- do not introduce Elasticsearch
- do not introduce a separate search engine
- use the relational database capabilities

Determine whether SQL Server `LIKE`, full-text search, or another approach is appropriate. Explain the recommendation.

---

## 30. API Versioning

Recommend whether the project should start with `/api/v1/...` or `/api/...`. Choose one approach. Explain the reasoning. Consider:

- this is a new project
- future breaking changes
- maintainability
- simplicity

Do not implement API versioning yet. Document the chosen approach.

---

## 31. Caching

Identify which endpoints could eventually be cached. For example:

- published articles
- categories
- tags
- home page data
- roadmap
- resources

Do not implement caching. Just document:

- what could be cached
- why
- possible cache duration
- whether invalidation is required

Keep caching as future optimization.

---

## 32. Security

Authentication and authorization are intentionally OUT OF SCOPE for this step. However, identify future protected operations. Examples:

```
POST   /articles
PUT    /articles/{id}
DELETE /articles/{id}

POST   /articles/{id}/publish
POST   /articles/{id}/archive

POST   /categories
PUT    /categories/{id}
DELETE /categories/{id}

POST   /tags
PUT    /tags/{id}
DELETE /tags/{id}
```

Mark them: **Authentication required — Future** / **Authorization required — Future**.

Public operations such as reading published articles should remain publicly accessible.

Also identify future security considerations:

- input validation
- HTML/content sanitization
- slug validation
- image URL validation
- rate limiting
- CORS
- API abuse
- authorization
- audit logging

Do not implement these yet.

---

## 33. Angular → API Mapping

Create a complete mapping table. Example:

| Angular Feature | Current Mock Method | Future API |
|---|---|---|
| Home | `getLatestArticles` | `GET /api/articles` |
| Blog list | `getAllPosts` | `GET /api/articles` |
| Blog detail | `getPostBySlug` | `GET /api/articles/slug/{slug}` |
| Admin articles | `getAllPosts` | `GET /api/articles` |
| Create | `createPost` | `POST /api/articles` |
| Edit | `updatePost` | `PUT /api/articles/{id}` |
| Delete | `deletePost` | `DELETE /api/articles/{id}` |
| Categories | `getAllCategories` | `GET /api/categories` |
| Tags | `getAllTags` | `GET /api/tags` |

**IMPORTANT:** Use the ACTUAL service method names found in:

```
src/app/core/services/blog-data.service.ts
```

Do not guess method names. Include every important Angular operation.

---

## 34. Identify Gaps

At the end of the document create:

```
## Frontend → Backend Gaps
```

List anything currently needed by Angular that the existing model/service does not represent cleanly. Examples:

- Article view count
- Article ordering
- Featured article selection
- Popular article calculation
- Image storage
- Author management
- Resource management
- Roadmap management
- Interview preparation management
- Slug history
- SEO metadata

For each gap classify it as:

- Required for V1
- Future
- Not required

Do not automatically implement a solution. Document the issue and recommendation.

---

## 35. Final Recommended V1 Backend Scope

At the end provide a clear recommendation.

**V1 Entities** — For example: Article, Category, Tag, ArticleTag

**V1 APIs** — List all required V1 endpoints.

**V1 DTOs** — List all request/response DTOs.

**V1 Database Relationships** — List them.

**Future Entities** — List them.

**Future APIs** — List them.

**Explicitly Out of Scope**

List:

- Authentication
- Authorization
- Image upload
- Analytics
- Advanced search
- Notifications
- Comments
- Likes

Only include items that are actually not part of the current project scope.

---

## 36. Important — No Backend Implementation

STOP after creating `docs/api-contract.md`.

Do NOT create:

- .NET solution
- .NET projects
- Controllers
- Services
- Repositories
- EF Core
- DbContext
- Entities
- Migrations
- SQL scripts

Do NOT connect Angular to the API.
Do NOT add authentication.
Do NOT add authorization.
Do NOT modify the frontend architecture just to make assumptions.

If a small Angular model clarification is absolutely required, document it first and make only the minimal change.

---

## 37. Final Response

After completing the analysis, report:

- Files inspected
- File created
- Persistent entities identified
- UI-only concepts identified
- Article model analysis
- ArticleBlock analysis
- Recommended content storage approach
- API endpoints
- Request DTOs
- Response DTOs
- Pagination contract
- Search contract
- Category contract
- Tag contract
- Slug contract
- Home page API requirements
- Resource requirements
- Roadmap requirements
- Interview preparation requirements
- Author requirements
- Database proposal
- Database relationships
- Indexes and unique constraints
- Error contract
- HTTP status codes
- API naming conventions
- Date/time conventions
- Image strategy
- API versioning recommendation
- Caching opportunities
- Authentication/authorization boundaries
- Angular → API mapping
- Frontend → Backend gaps
- Recommended V1 backend scope
- Future scope
- Open questions
- Confirmation that backend implementation was NOT started
