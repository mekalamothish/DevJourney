import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { ApiResponse, PaginatedResponse, AuthorQuery } from '../models/api-response.model';
import { AuthorRequest } from '../models/api-request.model';
import { Author } from '../models/blog-post';

/**
 * Author API Service
 * Handles all HTTP communication with the backend /authors endpoints.
 */
@Injectable({
  providedIn: 'root',
})
export class AuthorApiService {
  private readonly apiUrl = `${API_CONFIG.baseUrl}/authors`;

  constructor(private http: HttpClient) {}

  /**
   * GET /authors
   * Fetch paginated list of authors
   */
  getAuthors(query?: AuthorQuery): Observable<PaginatedResponse<Author>> {
    let params = new HttpParams();

    if (query) {
      if (query.page !== undefined) params = params.set('page', query.page.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
    }

    return this.http.get<PaginatedResponse<Author>>(this.apiUrl, { params });
  }

  /**
   * GET /authors/{id}
   * Fetch a single author by ID
   */
  getAuthorById(id: number): Observable<ApiResponse<Author>> {
    return this.http.get<ApiResponse<Author>>(`${this.apiUrl}/${id}`);
  }

  /**
   * POST /authors
   * Create a new author
   */
  createAuthor(request: AuthorRequest): Observable<ApiResponse<Author>> {
    return this.http.post<ApiResponse<Author>>(this.apiUrl, request);
  }

  /**
   * PUT /authors/{id}
   * Full update of an author
   */
  updateAuthor(id: number, request: AuthorRequest): Observable<ApiResponse<Author>> {
    return this.http.put<ApiResponse<Author>>(`${this.apiUrl}/${id}`, request);
  }
}
