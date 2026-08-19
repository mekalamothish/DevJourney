/**
 * Single source of truth for branding, navigation, and footer links.
 * Swap the placeholder brand here once a final name is chosen — nothing
 * else in the app should hard-code "DevJourney" or these links directly.
 */

export interface NavLink {
  label: string;
  /** Router path, e.g. '/blog'. Use '/' + fragment for same-page anchors. For external links, a full URL. */
  path: string;
  fragment?: string;
  external?: boolean;
}

export interface SocialLink {
  label: string;
  url: string;
  icon: 'github' | 'linkedin' | 'youtube';
}

const primaryNav: NavLink[] = [
  { label: 'Home', path: '/' },
  { label: 'Articles', path: '/blog' },
  { label: 'Topics', path: '/topics' },
  { label: 'Roadmap', path: '/roadmap' },
  { label: 'Resources', path: '/', fragment: 'resources' },
  { label: 'About', path: '/about' },
];

const headerCta: NavLink = { label: 'Start Learning', path: '/roadmap' };

const footerNavigation: NavLink[] = [
  { label: 'Home', path: '/' },
  { label: 'Articles', path: '/blog' },
  { label: 'Topics', path: '/topics' },
  { label: 'Roadmap', path: '/roadmap' },
  { label: 'About', path: '/about' },
];

/** Footer "Topics" column — a handful of the busiest categories, not the full list. */
const footerTopics: NavLink[] = [
  { label: 'C#', path: '/category/csharp' },
  { label: '.NET', path: '/category/dotnet' },
  { label: 'Angular', path: '/category/angular' },
  { label: 'SQL', path: '/category/sql' },
  { label: 'Azure', path: '/category/azure' },
];

const footerResources: NavLink[] = [
  { label: 'Projects', path: '/', fragment: 'resources' },
  { label: 'Interview Preparation', path: '/', fragment: 'interview-prep' },
  { label: 'GitHub', path: 'https://github.com/', external: true },
];

const social: SocialLink[] = [
  { label: 'GitHub', url: 'https://github.com/', icon: 'github' },
  { label: 'LinkedIn', url: 'https://linkedin.com/', icon: 'linkedin' },
  { label: 'YouTube', url: 'https://youtube.com/', icon: 'youtube' },
];

export const SITE_CONFIG = {
  name: 'DevJourney',
  tagline: 'Learn. Build. Share.',
  description:
    "Practical notes from one developer's path through C#, .NET, Angular, SQL, and system design — written down so the next pass through is faster.",
  url: 'https://devjourney.example.com',
  primaryNav,
  headerCta,
  footer: {
    navigation: footerNavigation,
    topics: footerTopics,
    resources: footerResources,
  },
  social,
};
