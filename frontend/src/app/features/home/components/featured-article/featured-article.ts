import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';

@Component({
  selector: 'dj-featured-article',
  imports: [RouterLink],
  templateUrl: './featured-article.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeaturedArticle {
  protected readonly post = signal<BlogPost | undefined>(undefined);

  constructor(private articleApi: ArticleApiService) {
    // Load first published article marked as featured
    this.articleApi.getArticles({ pageSize: 100, status: 'published' }).subscribe({
      next: (response) => {
        const featured = response.data.find(a => a.isFeatured);
        this.post.set(featured);
      },
      error: () => {
        this.post.set(undefined);
      }
    });
  }
}

