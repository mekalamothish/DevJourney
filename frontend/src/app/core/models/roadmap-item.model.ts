export interface RoadmapItem {
  id: string;
  slug: string;
  title: string;
  description: string;
  /** 1-based position along the roadmap */
  order: number;
  relatedCategorySlug?: string;
}
