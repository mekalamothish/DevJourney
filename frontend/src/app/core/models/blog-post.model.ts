import { Author } from './author.model';
import { BlogImage } from './blog-image.model';
import { Category } from './category.model';
import { BlogContentBlock } from './content-block.model';
import { Tag } from './tag.model';

export type BlogPostStatus = 'draft' | 'published';

export interface BlogPost {
  id: string;
  slug: string;
  title: string;
  /** Short dek shown on cards and at the top of the article */
  description: string;
  category: Category;
  tags: Tag[];
  author: Author;
  /** ISO 8601 date string */
  publishedAt: string;
  /** ISO 8601 date string, present only when the post was revised after publishing */
  updatedAt?: string;
  readingTimeMinutes: number;
  featuredImage: BlogImage;
  content: BlogContentBlock[];
  status: BlogPostStatus;
  /** Drives the homepage "Featured Article" section */
  featured?: boolean;
  /** Drives the homepage/blog "Popular Articles" section */
  popular?: boolean;
}
