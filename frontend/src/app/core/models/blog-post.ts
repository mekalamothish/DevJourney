export interface Author {
  id: number;
  name: string;
  avatar?: string;
  role?: string;
}

export interface Category {
  id: number;
  name: string;
  slug: string;
}

export interface Tag {
  id: number;
  name: string;
  slug: string;
}

// Structured article content blocks
export type ArticleBlock =
  | ParagraphBlock
  | HeadingBlock
  | SubheadingBlock
  | ListBlock
  | CodeBlock
  | TerminalBlock
  | QuoteBlock
  | CalloutBlock
  | TableBlock
  | ImageBlock
  | TakeawaysBlock
  | FaqBlock;

export interface SubheadingBlock {
  type: 'subheading';
  id: string;
  text: string;
}

export interface ParagraphBlock {
  type: 'paragraph';
  text: string;
}

export interface HeadingBlock {
  type: 'heading';
  level: 2 | 3;
  id: string;
  text: string;
}

export interface ListBlock {
  type: 'list';
  ordered?: boolean;
  items: string[];
}

export interface CodeBlock {
  type: 'code';
  language?: string;
  code: string;
  filename?: string;
}

export interface TerminalBlock {
  type: 'terminal';
  lines: string[];
}

export interface QuoteBlock {
  type: 'quote';
  text: string;
  author?: string;
}

export interface CalloutBlock {
  type: 'callout';
  variant: 'note' | 'tip' | 'warning' | 'important';
  heading?: string;
  text: string;
}

export interface TableBlock {
  type: 'table';
  headers: string[];
  rows: string[][];
}

export interface ImageBlock {
  type: 'image';
  src?: string;
  alt: string;
  caption?: string;
}

export interface TakeawaysBlock {
  type: 'takeaways';
  items: string[];
}

export interface FaqItem {
  q: string;
  a: string;
}

export interface FaqBlock {
  type: 'faq';
  items: FaqItem[];
}

export interface BlogPost {
  id: number;
  title: string;
  slug: string;
  excerpt: string;
  category: Category;
  tags: Tag[];
  publishedAt: string; // ISO date
  readingTime: number; // minutes
  featuredImage?: string;
  author: Author;
  isFeatured?: boolean;
  isPopular?: boolean;

  // Optional structured content for the full article
  content?: ArticleBlock[];

  // Admin properties
  status?: 'draft' | 'published' | 'archived';
  createdAt?: string;
  updatedAt?: string;
}
