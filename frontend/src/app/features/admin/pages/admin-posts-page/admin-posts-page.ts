import { ChangeDetectionStrategy, Component, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';

@Component({
  selector: 'dj-admin-posts-page',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './admin-posts-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminPostsPage {
  searchTerm = signal('');
  selectedStatus = signal<'all' | 'draft' | 'published' | 'archived'>('all');
  isLoading = signal(false);
  error = signal<string | undefined>(undefined);
  articles = signal<BlogPost[]>([]);

  constructor(private router: Router, private articleApi: ArticleApiService) {
    effect(() => {
      this.loadArticles();
    });
  }

  private loadArticles() {
    this.isLoading.set(true);
    this.error.set(undefined);

    const query: any = { pageSize: 100 };
    const status = this.selectedStatus();
    if (status !== 'all') {
      query.status = status;
    }
    const search = this.searchTerm().trim();
    if (search) {
      query.q = search;
    }

    this.articleApi.getArticles(query).subscribe({
      next: (response) => {
        this.articles.set(response.data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load articles:', err);
        this.error.set('Failed to load articles');
        this.articles.set([]);
        this.isLoading.set(false);
      }
    });
  }

  filteredPosts = computed(() => {
    return this.articles();
  });

  delete(p: BlogPost) {
    if (!confirm('Delete this post?')) return;
    this.articleApi.deleteArticle(p.id).subscribe({
      next: () => {
        this.articles.update(articles => articles.filter(a => a.id !== p.id));
      },
      error: (err) => {
        console.error('Failed to delete article:', err);
        alert('Failed to delete article');
      }
    });
  }

  togglePublish(p: BlogPost) {
    const isPublished = p.status === 'published';
    const operation = isPublished 
      ? this.articleApi.unpublishArticle(p.id)
      : this.articleApi.publishArticle(p.id);

    operation.subscribe({
      next: (response) => {
        this.articles.update(articles =>
          articles.map(a => a.id === p.id ? response.data : a)
        );
      },
      error: (err) => {
        console.error('Failed to toggle publish:', err);
        alert('Failed to update article status');
      }
    });
  }
}
