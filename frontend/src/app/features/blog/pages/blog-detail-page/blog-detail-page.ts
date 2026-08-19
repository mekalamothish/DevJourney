import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';
import { ArticleHeader } from '../../components/article-header/article-header';
import { ArticleContent } from '../../components/article-content/article-content';
import { ArticleToc } from '../../components/article-toc/article-toc';
import { RelatedArticles } from '../../components/related-articles/related-articles';
import { ArticleAuthor } from '../../components/article-author/article-author';
import { ArticleNavigation } from '../../components/article-navigation/article-navigation';
import { ArticleShare } from '../../components/article-share/article-share';

@Component({
  selector: 'dj-blog-detail-page',
  imports: [RouterLink, ArticleHeader, ArticleContent, ArticleToc, RelatedArticles, ArticleAuthor, ArticleNavigation, ArticleShare],
  templateUrl: './blog-detail-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlogDetailPage {
  protected readonly post = signal<BlogPost | undefined>(undefined);
  protected readonly related = signal<BlogPost[]>([]);
  protected readonly prev = signal<BlogPost | undefined>(undefined);
  protected readonly next = signal<BlogPost | undefined>(undefined);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | undefined>(undefined);

  constructor(private articleApi: ArticleApiService, route: ActivatedRoute) {
    const slug = route.snapshot.paramMap.get('slug') || '';
    if (!slug) {
      this.isLoading.set(false);
      this.error.set('Invalid article URL');
      return;
    }

    // Load article by slug
    this.articleApi.getArticleBySlug(slug).subscribe({
      next: (response) => {
        const article = response.data;
        this.post.set(article);
        this.isLoading.set(false);
        this.loadRelatedArticles(article);
        this.loadPrevNext(article);
      },
      error: (err) => {
        console.error('Failed to load article:', err);
        this.isLoading.set(false);
        this.error.set('Article not found');
      }
    });
  }

  private loadRelatedArticles(article: BlogPost) {
    // Load articles in same category (excluding current)
    this.articleApi.getArticles({ category: article.category.id, pageSize: 5, status: 'published' }).subscribe({
      next: (response) => {
        const related = response.data
          .filter(a => a.id !== article.id)
          .slice(0, 3);
        this.related.set(related);
      },
      error: () => {
        // Silently fail
        this.related.set([]);
      }
    });
  }

  private loadPrevNext(article: BlogPost) {
    // Get all articles in same category to compute prev/next
    this.articleApi.getArticles({ category: article.category.id, pageSize: 100, status: 'published' }).subscribe({
      next: (response) => {
        const sorted = response.data.sort((a, b) => 
          new Date(b.publishedAt || '').getTime() - new Date(a.publishedAt || '').getTime()
        );
        
        const index = sorted.findIndex(a => a.id === article.id);
        if (index > 0) this.prev.set(sorted[index - 1]);
        if (index < sorted.length - 1) this.next.set(sorted[index + 1]);
      },
      error: () => {
        // Silently fail
      }
    });
  }
}
