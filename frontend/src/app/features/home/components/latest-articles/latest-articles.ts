import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { BlogCard } from '../../../../shared/components/blog-card/blog-card';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';

@Component({
  selector: 'dj-latest-articles',
  imports: [BlogCard],
  templateUrl: './latest-articles.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LatestArticles {
  protected readonly posts = signal<BlogPost[]>([]);

  constructor(private articleApi: ArticleApiService) {
    // Load latest 6 published articles
    this.articleApi.getArticles({ pageSize: 6, status: 'published', sort: 'desc' }).subscribe({
      next: (response) => {
        this.posts.set(response.data);
      },
      error: () => {
        this.posts.set([]);
      }
    });
  }
}

