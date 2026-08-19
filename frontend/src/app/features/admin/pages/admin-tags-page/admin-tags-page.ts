import { ChangeDetectionStrategy, Component, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TagApiService } from '../../../../core/services/tag-api.service';

@Component({
  selector: 'dj-admin-tags-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-tags-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminTagsPage {
  tags = signal<any[]>([]);
  isLoading = signal(false);
  isSaving = signal(false);
  error = signal<string | undefined>(undefined);
  showNew = signal(false);
  newName = signal('');
  newSlug = signal('');

  constructor(private tagApi: TagApiService) {
    effect(() => {
      this.loadTags();
    });
  }

  private loadTags() {
    this.isLoading.set(true);
    this.error.set(undefined);

    this.tagApi.getTags({ pageSize: 100 }).subscribe({
      next: (response) => {
        this.tags.set(response.data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load tags:', err);
        this.error.set('Failed to load tags');
        this.tags.set([]);
        this.isLoading.set(false);
      }
    });
  }

  countArticles(t: any) {
    return 0;
  }

  create() {
    if (!this.newName() || !this.newSlug()) return alert('Name and slug required');
    this.isSaving.set(true);
    this.error.set(undefined);

    this.tagApi.createTag({
      name: this.newName(),
      slug: this.newSlug()
    }).subscribe({
      next: (response) => {
        this.tags.update(tgs => [...tgs, response.data]);
        this.showNew.set(false);
        this.newName.set('');
        this.newSlug.set('');
        this.isSaving.set(false);
      },
      error: (err) => {
        console.error('Failed to create tag:', err);
        this.error.set('Failed to create tag');
        this.isSaving.set(false);
      }
    });
  }

  cancel() {
    this.showNew.set(false);
    this.newName.set('');
    this.newSlug.set('');
  }

  edit(t: any) {
    const name = prompt('Tag name', t.name);
    if (!name) return;
    this.tagApi.updateTag(t.id, {
      name,
      slug: name.toLowerCase().replace(/\s+/g, '-')
    }).subscribe({
      next: (response) => {
        this.tags.update(tgs =>
          tgs.map(tag => tag.id === t.id ? response.data : tag)
        );
      },
      error: (err) => {
        console.error('Failed to update tag:', err);
        alert('Failed to update tag');
      }
    });
  }

  remove(t: any) {
    if (!confirm('Delete tag?')) return;
    this.tagApi.deleteTag(t.id).subscribe({
      next: () => {
        this.tags.update(tgs => tgs.filter(tag => tag.id !== t.id));
      },
      error: (err) => {
        console.error('Failed to delete tag:', err);
        alert('Failed to delete tag');
      }
    });
  }
}
