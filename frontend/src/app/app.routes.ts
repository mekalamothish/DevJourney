import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/public-layout/public-layout').then((m) => m.PublicLayout),
    children: [
      {
        path: '',
        title: 'DevJourney — Learn. Build. Share.',
        loadComponent: () =>
          import('./features/home/pages/home-page/home-page').then((m) => m.HomePage),
      },
      {
        path: 'blog',
        title: 'Articles — DevJourney',
        loadComponent: () =>
          import('./features/blog/pages/blog-list-page/blog-list-page').then((m) => m.BlogListPage),
      },
      {
        path: 'blog/:slug',
        loadComponent: () =>
          import('./features/blog/pages/blog-detail-page/blog-detail-page').then(
            (m) => m.BlogDetailPage,
          ),
      },
      {
        path: 'category/:slug',
        loadComponent: () =>
          import('./features/category/pages/category-page/category-page').then(
            (m) => m.CategoryPage,
          ),
      },
      {
        path: 'topics',
        title: 'Topics — DevJourney',
        loadComponent: () =>
          import('./features/topics/pages/topics-page/topics-page').then((m) => m.TopicsPage),
      },
      {
        path: 'search',
        title: 'Search — DevJourney',
        loadComponent: () =>
          import('./features/search/pages/search-page/search-page').then((m) => m.SearchPage),
      },
      {
        path: 'roadmap',
        title: 'Learning Roadmap — DevJourney',
        loadComponent: () =>
          import('./features/roadmap/pages/roadmap-page/roadmap-page').then((m) => m.RoadmapPage),
      },
      {
        path: 'resources',
        title: 'Resources — DevJourney',
        loadComponent: () =>
          import('./features/resources/pages/resources-page/resources-page').then((m) => m.ResourcesPage),
      },
      {
        path: 'about',
        title: 'About — DevJourney',
        loadComponent: () =>
          import('./features/about/pages/about-page/about-page').then((m) => m.AboutPage),
      },
    ],
  },
  {
    // Kept separate from the public layout tree (Section 24) so an
    // AdminLayout + route guards can be dropped in later without touching
    // the public routes above.
    path: 'admin',
    loadComponent: () => import('./layout/admin-layout/admin-layout').then((m) => m.AdminLayout),
    children: [
      {
        path: '',
        title: 'Admin — DevJourney',
        loadComponent: () =>
          import('./features/admin/pages/admin-dashboard-page/admin-dashboard-page').then(
            (m) => m.AdminDashboardPage,
          ),
      },
      {
        path: 'posts',
        title: 'Posts — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-posts-page/admin-posts-page').then(
            (m) => m.AdminPostsPage,
          ),
      },
      {
        path: 'posts/new',
        title: 'New Post — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-post-editor-page/admin-post-editor-page').then(
            (m) => m.AdminPostEditorPage,
          ),
      },
      {
        path: 'posts/:id/edit',
        title: 'Edit Post — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-post-editor-page/admin-post-editor-page').then(
            (m) => m.AdminPostEditorPage,
          ),
      },
      // Articles routes (alias of posts pages)
      {
        path: 'articles',
        title: 'Articles — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-posts-page/admin-posts-page').then(
            (m) => m.AdminPostsPage,
          ),
      },
      {
        path: 'articles/new',
        title: 'New Article — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-post-editor-page/admin-post-editor-page').then(
            (m) => m.AdminPostEditorPage,
          ),
      },
      {
        path: 'articles/:id/edit',
        title: 'Edit Article — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-post-editor-page/admin-post-editor-page').then(
            (m) => m.AdminPostEditorPage,
          ),
      },
      {
        path: 'articles/:id/preview',
        title: 'Preview Article — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-article-preview-page/admin-article-preview-page').then(
            (m) => m.AdminArticlePreviewPage,
          ),
      },
      {
        path: 'categories',
        title: 'Categories — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-categories-page/admin-categories-page').then(
            (m) => m.AdminCategoriesPage,
          ),
      },
      {
        path: 'tags',
        title: 'Tags — Admin',
        loadComponent: () =>
          import('./features/admin/pages/admin-tags-page/admin-tags-page').then(
            (m) => m.AdminTagsPage,
          ),
      },
    ],
  },

  {
    path: '**',
    title: 'Page not found — DevJourney',
    loadComponent: () => import('./shared/components/not-found/not-found').then((m) => m.NotFound),
  },
];
