/**
 * API DTOs for article requests.
 * Matches the V1 contract: uses IDs for relationships, not expanded objects.
 */

import { ArticleBlock } from './blog-post';

/**
 * Article creation DTO
 * Backend expects IDs for relationships: authorId, categoryId, tagIds
 * NOT expanded author/category/tags objects
 */
export interface ArticleCreateRequest {
  title: string;
  slug?: string;
  excerpt: string;
  featuredImage?: string;
  readingTime?: number;
  status?: 'draft' | 'published' | 'archived';
  publishedAt?: string;
  authorId: number;
  categoryId: number;
  tagIds?: number[];
  content: ArticleBlock[];
}

/**
 * Article update DTO
 * Same structure as create; used for both PUT (full) and PATCH (partial)
 */
export interface ArticleUpdateRequest {
  title?: string;
  slug?: string;
  excerpt?: string;
  featuredImage?: string;
  readingTime?: number;
  status?: 'draft' | 'published' | 'archived';
  publishedAt?: string;
  authorId?: number;
  categoryId?: number;
  tagIds?: number[];
  content?: ArticleBlock[];
}

/**
 * Category create/update DTO
 */
export interface CategoryRequest {
  name: string;
  slug?: string;
}

/**
 * Tag create/update DTO
 */
export interface TagRequest {
  name: string;
  slug?: string;
}

/**
 * Author create/update DTO
 */
export interface AuthorRequest {
  name: string;
  avatar?: string;
  role?: string;
  bio?: string;
}
