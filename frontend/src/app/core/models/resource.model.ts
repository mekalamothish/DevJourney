export type ResourceType =
  'project' | 'github' | 'cheatsheet' | 'tool' | 'exercise' | 'architecture-example';

export interface Resource {
  id: string;
  title: string;
  description: string;
  type: ResourceType;
  url: string;
}
