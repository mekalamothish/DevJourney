Validated API Contract — DevJourney Frontend vs docs/api-contract.md

Summary
-------
The existing frontend largely matches docs/api-contract.md. The contract is valid with a few clarifications and small mismatches that must be resolved before backend implementation. Below are findings, required clarifications, and recommended adjustments to the contract to align with the real code.

1) Matches (no change needed)
- Primary entities: Article, Category, Tag, Author, Resource (media) — frontend uses these shapes and the contract defines them.
- Article status enum: 'draft'|'published'|'archived' — used consistently in mock service and admin UI.
- Article content model: ArticleBlock[] union (paragraph, heading, list, code, callout, table, image, takeaways, faq) — supported by frontend renderer and editor.
- IDs are integers; pagination meta shape and endpoints described in the contract fit frontend needs.
- DTO shape: ArticleDTO in contract covers fields the frontend expects (id, title, slug, excerpt, featuredImage, readingTime, status, createdAt, updatedAt, publishedAt, author, category, tags, content).

2) Important mismatches and required clarifications
A. Terminal block property name
- Frontend model (src/app/core/models/blog-post.ts) defines TerminalBlock as: { type: 'terminal'; lines: string[] }
- renderer template (src/.../article-content.html) currently expects $any(b).commands (string) when rendering terminal blocks.
- Recommendation (validated contract): standardize on lines: string[] in contract. Rendering guidance: server returns TerminalBlock.lines array; frontend should render by joining lines with "\n" (or renderer can accept either lines:string[] or commands:string). Backend contract: TerminalBlock.lines: string[] (required) and optionally commands:string (deprecated). Update docs/api-contract.md to reflect lines.

B. Slug, author, category, tags payload shapes on create/update
- Frontend mock createPost/updatePost use nested objects (category object, author object, tags array of objects) in BlogPost Partial.
- Contract currently recommends ArticleCreateDTO to accept categoryId, authorId, tagIds (IDs). This is the preferred API design, but the frontend (mock) may post nested objects during development.
- Recommendation: Contract should state API accepts either:
  - canonical form: categoryId (number), authorId (number), tagIds:number[] (server preferred), or
  - temporary convenience: category:{id,name,slug} and tags:[{id,name,slug}] (server may accept and extract IDs)
- Backend implementers: support both or request frontend change later. Document this in validated contract.

C. Date formats and granularity
- Frontend mock uses date strings truncated to YYYY-MM-DD in many places (createdAt/publishedAt/updatedAt using .slice(0,10)).
- Contract currently requires ISO 8601 UTC datetimes (with times).
- Recommendation: Backend should accept full ISO 8601 datetimes and may also accept date-only strings. Backend must normalize and store full ISO datetime (UTC). Document accepted input formats.

D. Preview behavior
- Frontend preview route (/admin/articles/:id/preview) renders client-side using in-memory data; it does NOT call a preview API.
- Contract mentions preview endpoints as optional. Mark preview endpoint as optional. If backend preview is desired later, document an admin-only preview endpoint; otherwise, no immediate backend requirement.

E. Terminal / Code block render expectations
- CodeBlock fields in model: language, filename, code — matches contract. No change.
- TableBlock, ImageBlock, FaqBlock shapes match contract.

F. Subheading block
- A SubheadingBlock type ('subheading') exists in the frontend models. Contract already lists a 'subheading' block; ensure this is included in the final spec. Confirm validation rules (id,text present).

G. readingTime derivation
- Frontend stores readingTime in mock posts but contract recommends server can compute it. Contract OK: backend may return readingTime (derived) and accept optional readingTime from admin clients.

3) Minor recommended contract edits (to docs/api-contract.md)
- TerminalBlock: use lines:string[] (required) and document rendering guidelines (join with newline).
- Document that ArticleCreate/Update may accept either id references (authorId, categoryId, tagIds) or nested objects for convenience; prefer ids in production.
- Accept date-only inputs but store normalized ISO datetimes. Add examples for both formats.
- Mark preview endpoint as optional; note frontend currently does client-side preview.
- Clarify media usage: ImageBlock.src may be a full public URL or a resource id reference; backend should resolve/validate both.

4) Status: publish/unpublish behaviour confirmation
- Frontend mock togglePublish sets publishedAt to YYYY-MM-DD and status accordingly. Contract's publish rules (server sets publishedAt when publishing) are valid and compatible. Backend must support publish/unpublish endpoints.

5) Actionable items for backend implementers (summary)
- Implement Article entity with JSON content storage matching ArticleBlock shapes (including subheading and terminal lines).
- Accept both id-based and object-based category/author/tag payloads (or document strict API if prefer one).
- Normalize date inputs to ISO 8601 UTC.
- Enforce slug uniqueness and return 409 on conflict.
- Media upload endpoint recommended; ImageBlock.src must be validated.
- By default, public GET /articles returns only published articles; admin endpoints require auth (future work).

Conclusion
----------
The current docs/api-contract.md is a solid basis. After applying the small clarifications above (terminal.lines, payload id vs object flexibility, date formats, preview note, media src semantics), the contract is validated against the codebase and ready for backend implementation.

I saved this validated contract as docs/api-contract-validated.md in the repo. If you want, I can apply the suggested edits directly to docs/api-contract.md or open a PR-style patch. Stopping now as requested.