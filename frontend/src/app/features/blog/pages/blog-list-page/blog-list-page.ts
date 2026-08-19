import { ChangeDetectionStrategy, Component, computed, effect, signal, DestroyRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BlogCard } from '../../../../shared/components/blog-card/blog-card';
import { FeaturedArticle } from '../../../home/components/featured-article/featured-article';
import { PopularArticles } from '../../../home/components/popular-articles/popular-articles';
import { BlogSearch } from '../../components/blog-search/blog-search';
import { CategoryFilter } from '../../components/category-filter/category-filter';
import { BlogPagination } from '../../components/blog-pagination/blog-pagination';
import { EmptyState } from '../../components/empty-state/empty-state';
import { BlogCardSkeleton } from '../../components/blog-card-skeleton/blog-card-skeleton';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import { CategoryApiService } from '../../../../core/services/category-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';
import type { PaginatedResponse } from '../../../../core/models/api-response.model';

@Component({
  selector: 'dj-blog-list-page',
  imports: [
    BlogCard,
    FeaturedArticle,
    PopularArticles,
    BlogSearch,
    CategoryFilter,
    BlogPagination,
    EmptyState,
    BlogCardSkeleton,
    ReactiveFormsModule,
  ],
  templateUrl: './blog-list-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlogListPage {
  private readonly pageSize = 9;

  protected readonly search = signal('');
  protected readonly selectedCategory = signal<string | undefined>(undefined);
  protected readonly currentPage = signal(1);
  protected readonly isLoading = signal(false);
  protected readonly error = signal<string | undefined>(undefined);

  protected readonly categories = signal<any[]>([]);
  protected readonly articles = signal<BlogPost[]>([]);
  protected readonly totalCount = signal(0);

  protected readonly searchControl = new FormControl('');

  constructor(
    private articleApi: ArticleApiService,
    private categoryApi: CategoryApiService,
    private router: Router,
    private route: ActivatedRoute,
    private destroyRef: DestroyRef
  ) {
    // Initialize from query params
    const qp = this.route.snapshot.queryParamMap;
    const q = qp.get('search') ?? '';
    const c = qp.get('category') ?? undefined;
    const p = Number(qp.get('page') ?? '1') || 1;
    this.search.set(q);
    this.selectedCategory.set(c ?? undefined);
    this.currentPage.set(p);

    // Load categories
    this.categoryApi.getCategories().subscribe({
      next: (response: PaginatedResponse<any>) => {
        this.categories.set(response.data.map(cat => cat.name));
      },
      error: () => {
        // Silently fail for categories, UI still works
        this.categories.set([]);
      }
    });

    // Initialize search control from query params without emitting valueChanges
    this.searchControl.setValue(q, { emitEvent: false });

    // Subscribe to control changes with debounce, update query params and reload
    this.searchControl.valueChanges.pipe(
      debounceTime(350),
      distinctUntilChanged(),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(v => {
      const val = (v ?? '') as string;
      this.search.set(val);
      this.currentPage.set(1);
      this.updateQueryParams();
      this.loadArticles();
    });

    // Load articles when category or page changes
    effect(() => {
      this.currentPage();
      this.selectedCategory();
      this.loadArticles();
    });

    // Initial load
    this.loadArticles();
  }

  private loadArticles() {
    this.isLoading.set(true);
    this.error.set(undefined);

    const query: any = {
      page: this.currentPage(),
      pageSize: this.pageSize,
      status: 'published',
    };

    if (this.search()) query.q = this.search();
    if (this.selectedCategory()) query.category = this.selectedCategory();

    this.articleApi.getArticles(query).subscribe({
      next: (response: PaginatedResponse<BlogPost>) => {
        console.log('API RESPONSE:', response);
        console.log('CATEGORIES:', response.data);
        this.articles.set(response.data);
        this.totalCount.set(response.meta.total);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load articles:', err);
        this.error.set('Failed to load articles. Please try again.');
        this.articles.set([]);
        this.isLoading.set(false);
      }
    });
  }

  protected readonly totalPages = computed(() => {
    const total = this.totalCount();
    return Math.max(1, Math.ceil(total / this.pageSize));
  });

  protected readonly paginated = computed(() => {
    return this.articles();
  });

  protected readonly filteredCount = computed(() => {
    return this.articles().length;
  });

  protected onCategory(cat?: string) {
    this.selectedCategory.set(cat);
    this.currentPage.set(1);
    this.updateQueryParams();
  }

  protected onPage(page: number) {
    this.currentPage.set(page);
    // scroll to the top of the articles list
    setTimeout(() => {
      const el = document.getElementById('article-list');
      if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 50);
    this.updateQueryParams();
  }

  protected clearFilters() {
    this.search.set('');
    this.selectedCategory.set(undefined);
    this.currentPage.set(1);
    this.updateQueryParams();
  }

  private updateQueryParams() {
    const qp: any = {};
    if (this.search()) qp.search = this.search();
    if (this.selectedCategory()) qp.category = this.selectedCategory();
    if (this.currentPage() && this.currentPage() > 1) qp.page = this.currentPage();

    this.router.navigate([], { relativeTo: this.route, queryParams: qp, replaceUrl: true });
  }

}

