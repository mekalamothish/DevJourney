import { ChangeDetectionStrategy, Component, signal, effect } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';
import { ArticleHeader } from '../../../blog/components/article-header/article-header';
import { ArticleContent } from '../../../blog/components/article-content/article-content';

@Component({
  selector: 'dj-admin-article-preview-page',
  standalone: true,
  imports: [CommonModule, ArticleHeader, ArticleContent],
  template: `
  <section class="p-6">
    @if (isLoading()) {
      <div>
        <p class="text-sm text-ink/60">Loading article...</p>
      </div>
    } @else if (error()) {
      <div class="rounded-md bg-red-50 p-4">
        <p class="text-sm text-red-800">{{ error() }}</p>
      </div>
    } @else if (!post()) {
      <h2 class="text-xl font-semibold">Article not found</h2>
    } @else {
      <dj-article-header [post]="post()!"></dj-article-header>
      <dj-article-content [blocks]="post()!.content || []"></dj-article-content>
    }
  </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminArticlePreviewPage {
  post = signal<BlogPost | undefined>(undefined);
  isLoading = signal(false);
  error = signal<string | undefined>(undefined);

  constructor(route: ActivatedRoute, private articleApi: ArticleApiService) {
    effect(() => {
      const id = route.snapshot.paramMap.get('id');
      if (!id) return;
      this.loadArticle(Number(id));
    });
  }

  private loadArticle(id: number) {
    this.isLoading.set(true);
    this.error.set(undefined);

    this.articleApi.getArticleById(id).subscribe({
      next: (response) => {
        this.post.set(response.data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load article:', err);
        this.error.set('Failed to load article');
        this.post.set(undefined);
        this.isLoading.set(false);
      }
    });
  }
}
