import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { ApiResponse, PaginatedResponse, CategoryQuery } from '../models/api-response.model';
import { CategoryRequest } from '../models/api-request.model';
import { Category } from '../models/blog-post';

/**
 * Category API Service
 * Handles all HTTP communication with the backend /categories endpoints.
 */
@Injectable({
  providedIn: 'root',
})
export class CategoryApiService {
  private readonly apiUrl = `${API_CONFIG.baseUrl}/categories`;

  constructor(private http: HttpClient) {}

  /**
   * GET /categories
   * Fetch paginated list of categories
   */
  getCategories(query?: CategoryQuery): Observable<PaginatedResponse<Category>> {
    let params = new HttpParams();

    if (query) {
      if (query.page !== undefined) params = params.set('page', query.page.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
      if (query.includeCounts !== undefined) params = params.set('includeCounts', query.includeCounts.toString());
    }

    return this.http.get<PaginatedResponse<Category>>(this.apiUrl, { params });
  }

  /**
   * GET /categories/{id}
   * Fetch a single category by ID
   */
  getCategoryById(id: number): Observable<ApiResponse<Category>> {
    return this.http.get<ApiResponse<Category>>(`${this.apiUrl}/${id}`);
  }

  /**
   * POST /categories
   * Create a new category
   */
  createCategory(request: CategoryRequest): Observable<ApiResponse<Category>> {
    return this.http.post<ApiResponse<Category>>(this.apiUrl, request);
  }

  /**
   * PUT /categories/{id}
   * Full update of a category
   */
  updateCategory(id: number, request: CategoryRequest): Observable<ApiResponse<Category>> {
    return this.http.put<ApiResponse<Category>>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * DELETE /categories/{id}
   * Delete a category
   */
  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
