import { BlogPost } from './blog-post.model';

export interface SearchResult {
  query: string;
  results: BlogPost[];
  totalResults: number;
}
