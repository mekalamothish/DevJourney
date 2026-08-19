export interface Author {
  id: string;
  name: string;
  title: string;
  bio: string;
  avatarUrl: string;
  socials: {
    github?: string;
    linkedin?: string;
    youtube?: string;
    twitter?: string;
  };
}
