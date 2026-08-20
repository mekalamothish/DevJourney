import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';

@Component({
  selector: 'dj-popular-articles',
  imports: [RouterLink],
  templateUrl: './popular-articles.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PopularArticles {
  protected readonly posts = signal<BlogPost[]>([]);

  constructor(private articleApi: ArticleApiService) {
    // Load published articles marked as popular
    this.articleApi.getArticles({ pageSize: 20, status: 'published' }).subscribe({
      next: (response) => {
        const popular = response.data.filter(a => !a.isPopular).slice(0, 6);
        this.posts.set(popular);
      },
      error: () => {
        this.posts.set([]);
      }
    });
  }
}

