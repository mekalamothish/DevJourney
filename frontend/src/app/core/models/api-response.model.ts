/**
 * API response types for ASP.NET Core backend.
 * Matches the FINAL V1 contract structures.
 */

/**
 * Single resource response envelope
 * Example: { "data": { id: 1, title: "..." } }
 */
export interface ApiResponse<T> {
  data: T;
}

/**
 * Paginated list response envelope
 * Example: { "data": [...], "meta": { total: 100, page: 1, pageSize: 20 } }
 */
export interface PaginatedResponse<T> {
  data: T[];
  meta: {
    total: number;
    page: number;
    pageSize: number;
  };
}

/**
 * Error response envelope from ASP.NET Core API
 * Example: { "error": { "code": "validation_error", "message": "...", "details": {} } }
 */
export interface ApiErrorResponse {
  error: {
    code: string;
    message: string;
    details?: Record<string, unknown>;
  };
}

/**
 * Query parameters for article list endpoint
 */
export interface ArticleQuery {
  page?: number;
  pageSize?: number;
  q?: string;
  category?: string | number;
  tag?: string | number;
  authorId?: number;
  status?: 'draft' | 'published' | 'archived';
  sort?: string;
  since?: string;
  until?: string;
}

/**
 * Query parameters for category list endpoint
 */
export interface CategoryQuery {
  page?: number;
  pageSize?: number;
  includeCounts?: boolean;
}

/**
 * Query parameters for tag list endpoint
 */
export interface TagQuery {
  page?: number;
  pageSize?: number;
  q?: string;
}

/**
 * Query parameters for author list endpoint
 */
export interface AuthorQuery {
  page?: number;
  pageSize?: number;
}
