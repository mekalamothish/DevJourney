export interface Category {
  id: string;
  slug: string;
  name: string;
  description: string;
  /** Small mono-style label shown on badges, e.g. "C#", ".NET" */
  shortLabel: string;
  articleCount: number;
}
