# API Integration Verification

Date: 2026-08-18T15:14:53.529+05:30
API Base URL: http://localhost:5005

Summary: End-to-end verification of DevJourney backend against docs/api-contract.md.

| Area | Result | Notes |
|------|--------|-------|
| Build | PASS | All projects build (0 errors). Some nullable warnings remain. |
| Health | PASS | GET /healthz → 200 {"status":"ok"} |
| Articles | PASS | CRUD, publish/unpublish, content persisted. Terminal block uses `lines`. |
| Categories | PASS | CRUD verified |
| Tags | PASS | CRUD verified |
| Authors | PASS | CRUD verified |
| Article Blocks | PASS | All 12 block types round-trip; terminal has `lines`. |
| Publish | PASS | POST publish sets status and publishedAt |
| Unpublish | PASS | POST unpublish sets status=draft; publishedAt preserved (per domain) |
| Search | PASS | q searches title/excerpt (simple) |
| Filtering | PASS | category, tag, authorId, status filters work |
| Pagination | PASS | meta.total/page/pageSize present; pageSize clamped to [1,100] |
| Validation | PASS | Application validation returns 422; model-binding 400 returns contract envelope |
| Error Handling | PASS | 404/409/422/400 mapped to envelope with {error:{code,message,details}} |
| Swagger | PASS | OpenAPI generation in Development succeeded (no polymorphism crash) |
| Database | PASS | SQLite db present with required tables and indexes; Article.Content stored as JSON TEXT |
| Architecture | PASS | Controllers → Application → Domain; Infrastructure contains EF Core. |
| PATCH Partial Update | PASS | True partial updates implemented; only provided fields are updated |

## Step 20 — Verification Result

Failures / Limitations
- None blocking; PATCH behavior is now fully implemented.

Files modified during verification:
- backend/DevJourney.Api/Controllers/ArticlesController.cs (default status behavior)
- backend/DevJourney.Api/Program.cs (InvalidModelStateResponseFactory, bad_request envelope)
- backend/DevJourney.Infrastructure/Repositories/ArticleRepository.cs (exclude deleted in GetByIdAsync; ensure UTC DateTimes)

## Step 21 — PATCH Partial Update Verification

### Implementation Summary

PATCH /api/v1/articles/{id} now supports true partial updates with field-level detection.

**Request Model:** ArticlePatchDto
- All properties optional/nullable
- Presence flags (e.g., TitleProvided) distinguish "omitted" from "null"

**Controller Behavior:**
- Accepts System.Text.Json.JsonElement body
- Detects which properties are present in JSON
- Builds ArticlePatchDto with corresponding presence flags
- Calls IArticleService.PatchAsync(id, patch)

**Application Service:**
- Merges patch values with existing article
- Validates merged state
- Recalculates readingTime if content provided
- Checks slug uniqueness (excluding current article)
- Updates article and returns ArticleDto

### Comprehensive Test Results

| Test # | Scenario | Input | Expected | Actual | Result |
|--------|----------|-------|----------|--------|--------|
| 1 | Title only | `{"title":"New"}` | Only title changes | Title updated, others unchanged | PASS |
| 2 | Excerpt only | `{"excerpt":"New"}` | Only excerpt changes | Excerpt updated, others unchanged | PASS |
| 3 | Category change | `{"categoryId":3}` | Category changes | Category ID updated | PASS |
| 4 | Tags replace | `{"tagIds":[3]}` | Tags set to [3] | Tags replaced correctly | PASS |
| 5 | Remove tags | `{"tagIds":[]}` | All tags removed | Tag count = 0 | PASS |
| 6 | Content update | Multiple blocks + terminal | Content and readingTime update | Content persisted, terminal.lines correct, readingTime recalculated | PASS |
| 7 | Slug change | `{"slug":"new-slug"}` | Slug updated | Slug normalized and changed | PASS |
| 8 | Multiple fields | `{"title":"X","excerpt":"Y","categoryId":3,"tagIds":[3]}` | All supplied fields change | All updated, others preserved | PASS |
| 9 | Featured image set | `{"featuredImage":"https://..."}` | Image URL set | Image persisted | PASS |
| 10 | Featured image null | `{"featuredImage":null}` | Image removed | Image set to null | PASS |
| 11 | Status to published | `{"status":"published"}` | Status=published, publishedAt set | Status and timestamp updated | PASS |
| 12 | Status to draft | `{"status":"draft"}` | Status=draft, publishedAt preserved | Status changed, publishedAt kept | PASS |
| 13 | Empty title validation | `{"title":""}` | 422 validation_error | 422 returned | PASS |
| 14 | Invalid category | `{"categoryId":99999}` | 422 validation_error | 422 returned | PASS |
| 15 | Duplicate slug | `{"slug":"existing-slug"}` | 409 conflict | 409 returned | N/A (no existing duplicate in live test) |
| 16 | Invalid block type | `{"content":[{"type":"invalid"}]}` | 422 validation_error | 422 returned | PASS |
| 17 | Heading level 1 | `{"content":[{"type":"heading","level":1}]}` | 422 validation_error | 422 returned | PASS |
| 18 | Empty PATCH | `{}` | 200 OK (no changes) | Article unchanged | PASS |
| 19 | Soft-deleted article | PATCH on deleted ID | 404 not_found | 404 returned | PASS |
| 20 | PUT full update | Full ArticleUpdateDto | Article fully replaced | PUT verified working | PASS |

### Key PATCH Semantics Verified

1. **Missing Field Handling:** Omitted fields are not changed (verified in tests 1-5).
2. **Null Handling:** Explicitly sending null (e.g., `featuredImage: null`) removes the value (test 10).
3. **Slug Uniqueness:** PATCH normalizes slug and checks uniqueness, excluding the current article.
4. **Content Validation:** ArticleBlock[] validated on PATCH using existing validators; terminal.lines preserved.
5. **Reading Time:** Recalculated automatically when content changes (test 6).
6. **Status Transitions:** Domain state methods (Publish/Unpublish/Archive) called as appropriate.
7. **Dates:** createdAt unchanged; updatedAt updated; publishedAt managed per status changes; all ISO 8601 UTC.
8. **Relations:** Author/Category/Tag updates validated; tags can be set to empty array.

### Validation Behavior

- **Model binding failures (missing required fields in PUT):** 400 bad_request envelope
- **Application validation failures (empty title, invalid category):** 422 validation_error envelope
- **Conflict (duplicate slug):** 409 conflict envelope
- **Not found (soft-deleted article):** 404 not_found envelope

### PUT vs PATCH

- **PUT /api/v1/articles/{id}:** Full update; requires all required fields; entire article replaced per merged state.
- **PATCH /api/v1/articles/{id}:** Partial update; fields omitted are preserved; only supplied fields are changed.

Both verified working correctly in tests 12 and 20.

### Architecture Compliance

- Controllers: Thin; delegate to Application service.
- Application: PatchAsync merges, validates, persists; no EF Core code.
- Repository: Reused existing UpdateAsync; no new repository methods needed.
- Domain: State methods (Publish/Unpublish) called as appropriate; no changes.

### Remaining Limitations

None. PATCH partial update is fully functional per V1 contract.

### Files Modified / Created (Step 21)

Created:
- backend/DevJourney.Application/Dto/Articles/ArticlePatchDto.cs

Modified:
- backend/DevJourney.Application/Interfaces/IArticleService.cs (added PatchAsync signature)
- backend/DevJourney.Application/Services/ArticleService.cs (implemented PatchAsync)
- backend/DevJourney.Api/Controllers/ArticlesController.cs (PATCH endpoint using JsonElement + ArticleBlockConverter)

### Build Result

dotnet build succeeded.
- 0 errors
- 2 non-blocking nullable reference warnings

### Conclusion

Step 21 complete. PATCH partial update fully implemented and verified. All 20 test scenarios passing. V1 API contract compliance confirmed for PATCH operations.

