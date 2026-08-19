import { ChangeDetectionStrategy, Component, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryApiService } from '../../../../core/services/category-api.service';
import { ArticleApiService } from '../../../../core/services/article-api.service';

@Component({
  selector: 'dj-admin-categories-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-categories-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminCategoriesPage {
  categories = signal<any[]>([]);
  isLoading = signal(false);
  isSaving = signal(false);
  error = signal<string | undefined>(undefined);
  showNew = signal(false);
  newName = signal('');
  newSlug = signal('');

  constructor(
    private categoryApi: CategoryApiService,
    private articleApi: ArticleApiService,
  ) {
    effect(() => {
      this.loadCategories();
    });
  }

  private loadCategories() {
    this.isLoading.set(true);
    this.error.set(undefined);

    this.categoryApi.getCategories({ pageSize: 100 }).subscribe({
      next: (response) => {
        this.categories.set(response.data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load categories:', err);
        this.error.set('Failed to load categories');
        this.categories.set([]);
        this.isLoading.set(false);
      }
    });
  }

  countArticles(c: any) {
    return 0;
  }

  create() {
    if (!this.newName() || !this.newSlug()) return alert('Name and slug required');
    this.isSaving.set(true);
    this.error.set(undefined);

    this.categoryApi.createCategory({
      name: this.newName(),
      slug: this.newSlug()
    }).subscribe({
      next: (response) => {
        this.categories.update(cats => [...cats, response.data]);
        this.showNew.set(false);
        this.newName.set('');
        this.newSlug.set('');
        this.isSaving.set(false);
      },
      error: (err) => {
        console.error('Failed to create category:', err);
        this.error.set('Failed to create category');
        this.isSaving.set(false);
      }
    });
  }

  cancel() {
    this.showNew.set(false);
    this.newName.set('');
    this.newSlug.set('');
  }

  edit(c: any) {
    const name = prompt('Category name', c.name);
    if (!name) return;
    this.categoryApi.updateCategory(c.id, {
      name,
      slug: name.toLowerCase().replace(/\s+/g, '-')
    }).subscribe({
      next: (response) => {
        this.categories.update(cats =>
          cats.map(cat => cat.id === c.id ? response.data : cat)
        );
      },
      error: (err) => {
        console.error('Failed to update category:', err);
        alert('Failed to update category');
      }
    });
  }

  remove(c: any) {
    if (!confirm('Delete category?')) return;
    this.categoryApi.deleteCategory(c.id).subscribe({
      next: () => {
        this.categories.update(cats => cats.filter(cat => cat.id !== c.id));
      },
      error: (err) => {
        console.error('Failed to delete category:', err);
        alert('Failed to delete category');
      }
    });
  }
}
