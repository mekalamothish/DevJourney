import { BlogImage } from './blog-image.model';

/**
 * Structured article body, block by block, instead of a single raw HTML blob.
 * This is what lets TableOfContentsComponent derive headings directly from
 * typed data (filter by type === 'heading') rather than parsing rendered DOM,
 * and lets each block type get its own typography treatment (Section 21).
 */
export type BlogContentBlock =
  | { type: 'heading'; level: 2 | 3 | 4; text: string; id: string }
  | { type: 'paragraph'; html: string }
  | { type: 'list'; ordered: boolean; items: string[] }
  | { type: 'blockquote'; html: string }
  | { type: 'code'; language: string; code: string; filename?: string }
  | { type: 'table'; headers: string[]; rows: string[][] }
  | { type: 'image'; image: BlogImage; caption?: string };
