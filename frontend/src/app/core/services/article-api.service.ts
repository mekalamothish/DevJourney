import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { ApiResponse, PaginatedResponse, ArticleQuery } from '../models/api-response.model';
import { ArticleCreateRequest, ArticleUpdateRequest } from '../models/api-request.model';
import { BlogPost } from '../models/blog-post';

/**
 * Article API Service
 * Handles all HTTP communication with the backend /articles endpoints.
 * All methods return Observables for the caller to subscribe to.
 */
@Injectable({
  providedIn: 'root',
})
export class ArticleApiService {
  private readonly apiUrl = `${API_CONFIG.baseUrl}/articles`;

  constructor(private http: HttpClient) {}

  /**
   * GET /articles
   * Fetch paginated list of articles with optional filters
   */
  getArticles(query?: ArticleQuery): Observable<PaginatedResponse<BlogPost>> {
    let params = new HttpParams();

    if (query) {
      if (query.page !== undefined) params = params.set('page', query.page.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
      if (query.q !== undefined) params = params.set('q', query.q);
      if (query.category !== undefined) params = params.set('category', query.category.toString());
      if (query.tag !== undefined) params = params.set('tag', query.tag.toString());
      if (query.authorId !== undefined) params = params.set('authorId', query.authorId.toString());
      if (query.status !== undefined) params = params.set('status', query.status);
      if (query.sort !== undefined) params = params.set('sort', query.sort);
      if (query.since !== undefined) params = params.set('since', query.since);
      if (query.until !== undefined) params = params.set('until', query.until);
    }

    return this.http.get<PaginatedResponse<BlogPost>>(this.apiUrl, { params });
  }

  /**
   * GET /articles/{id}
   * Fetch a single article by ID
   */
  getArticleById(id: number): Observable<ApiResponse<BlogPost>> {
    return this.http.get<ApiResponse<BlogPost>>(`${this.apiUrl}/${id}`);
  }

  /**
   * GET /articles/slug/{slug}
   * Fetch a single article by slug
   */
  getArticleBySlug(slug: string): Observable<ApiResponse<BlogPost>> {
    return this.http.get<ApiResponse<BlogPost>>(`${this.apiUrl}/slug/${slug}`);
  }

  /**
   * POST /articles
   * Create a new article
   */
  createArticle(request: ArticleCreateRequest): Observable<ApiResponse<BlogPost>> {
    return this.http.post<ApiResponse<BlogPost>>(this.apiUrl, request);
  }

  /**
   * PUT /articles/{id}
   * Full update of an article (all fields required)
   */
  updateArticle(id: number, request: ArticleUpdateRequest): Observable<ApiResponse<BlogPost>> {
    return this.http.put<ApiResponse<BlogPost>>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * PATCH /articles/{id}
   * Partial update of an article (only provided fields are updated)
   */
  patchArticle(id: number, request: Partial<ArticleUpdateRequest>): Observable<ApiResponse<BlogPost>> {
    return this.http.patch<ApiResponse<BlogPost>>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * DELETE /articles/{id}
   * Soft delete an article
   */
  deleteArticle(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * POST /articles/{id}/publish
   * Publish an article (transition from draft/archived to published)
   */
  publishArticle(id: number): Observable<ApiResponse<BlogPost>> {
    return this.http.post<ApiResponse<BlogPost>>(`${this.apiUrl}/${id}/publish`, {});
  }

  /**
   * POST /articles/{id}/unpublish
   * Unpublish an article (transition from published to draft)
   */
  unpublishArticle(id: number): Observable<ApiResponse<BlogPost>> {
    return this.http.post<ApiResponse<BlogPost>>(`${this.apiUrl}/${id}/unpublish`, {});
  }
}
