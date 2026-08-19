import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { ApiResponse, PaginatedResponse, TagQuery } from '../models/api-response.model';
import { TagRequest } from '../models/api-request.model';
import { Tag } from '../models/blog-post';

/**
 * Tag API Service
 * Handles all HTTP communication with the backend /tags endpoints.
 */
@Injectable({
  providedIn: 'root',
})
export class TagApiService {
  private readonly apiUrl = `${API_CONFIG.baseUrl}/tags`;

  constructor(private http: HttpClient) {}

  /**
   * GET /tags
   * Fetch paginated list of tags
   */
  getTags(query?: TagQuery): Observable<PaginatedResponse<Tag>> {
    let params = new HttpParams();

    if (query) {
      if (query.page !== undefined) params = params.set('page', query.page.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
      if (query.q !== undefined) params = params.set('q', query.q);
    }

    return this.http.get<PaginatedResponse<Tag>>(this.apiUrl, { params });
  }

  /**
   * GET /tags/{id}
   * Fetch a single tag by ID
   */
  getTagById(id: number): Observable<ApiResponse<Tag>> {
    return this.http.get<ApiResponse<Tag>>(`${this.apiUrl}/${id}`);
  }

  /**
   * POST /tags
   * Create a new tag
   */
  createTag(request: TagRequest): Observable<ApiResponse<Tag>> {
    return this.http.post<ApiResponse<Tag>>(this.apiUrl, request);
  }

  /**
   * PUT /tags/{id}
   * Full update of a tag
   */
  updateTag(id: number, request: TagRequest): Observable<ApiResponse<Tag>> {
    return this.http.put<ApiResponse<Tag>>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * DELETE /tags/{id}
   * Delete a tag
   */
  deleteTag(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
