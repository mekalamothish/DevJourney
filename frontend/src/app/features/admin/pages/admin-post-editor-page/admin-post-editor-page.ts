import { ChangeDetectionStrategy, Component, signal, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ArticleContent } from '../../../blog/components/article-content/article-content';
import { RichTextEditorComponent } from '../../../../shared/rich-text-editor/rich-text-editor.component';
import { RouterLink } from '@angular/router';
import { ArticleApiService } from '../../../../core/services/article-api.service';
import { CategoryApiService } from '../../../../core/services/category-api.service';
import { TagApiService } from '../../../../core/services/tag-api.service';
import { AuthorApiService } from '../../../../core/services/author-api.service';
import type { BlogPost } from '../../../../core/models/blog-post';

interface EditorModel {
  id?: number;
  title: string;
  slug: string;
  excerpt: string;
  featuredImage?: string;
  content: any[];
  status?: string;
  publishedAt?: string;
  readingTime?: number;
  authorId: number;
  categoryId: number;
  tagIds: number[];
  author?: { id: number; name: string; avatar?: string };
  category?: { id: number; name: string; slug: string };
  tags?: { id: number; name: string; slug: string }[];
}

@Component({
  selector: 'dj-admin-post-editor-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, ArticleContent, RichTextEditorComponent],
  templateUrl: './admin-post-editor-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminPostEditorPage {
  isLoading = signal(false);
  isSaving = signal(false);
  error = signal<string | undefined>(undefined);
  
  model = signal<EditorModel>({
    title: '',
    slug: '',
    excerpt: '',
    content: [],
    authorId: 1,
    categoryId: 1,
    tagIds: [],
  });

  categories = signal<any[]>([]);
  authors = signal<any[]>([]);
  allTags = signal<any[]>([]);

  newBlockType = signal('paragraph');
  tagsCsv = signal('');
  editingId?: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private articleApi: ArticleApiService,
    private categoryApi: CategoryApiService,
    private tagApi: TagApiService,
    private authorApi: AuthorApiService,
  ) {
    effect(() => {
      this.loadDropdownData();
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const num = Number(id);
      this.editingId = num;
      this.loadArticle(num);
    }
  }

  private loadDropdownData() {
    this.categoryApi.getCategories({ pageSize: 100 }).subscribe({
      next: (response) => {
        this.categories.set(response.data);
      },
      error: (err) => {
        console.error('Failed to load categories:', err);
      }
    });

    this.authorApi.getAuthors({ pageSize: 100 }).subscribe({
      next: (response) => {
        this.authors.set(response.data);
      },
      error: (err) => {
        console.error('Failed to load authors:', err);
      }
    });

    this.tagApi.getTags({ pageSize: 100 }).subscribe({
      next: (response) => {
        this.allTags.set(response.data);
      },
      error: (err) => {
        console.error('Failed to load tags:', err);
      }
    });
  }

  private loadArticle(id: number) {
    this.isLoading.set(true);
    this.error.set(undefined);

    this.articleApi.getArticleById(id).subscribe({
      next: (response) => {
        const article = response.data;
        const model: EditorModel = {
          id: article.id,
          title: article.title,
          slug: article.slug,
          excerpt: article.excerpt,
          featuredImage: article.featuredImage,
          content: article.content || [],
          status: article.status,
          publishedAt: article.publishedAt,
          readingTime: article.readingTime,
          authorId: article.author.id,
          categoryId: article.category.id,
          tagIds: (article.tags || []).map(t => t.id),
          author: article.author,
          category: article.category,
          tags: article.tags,
        };
        this.model.set(model);
        this.tagsCsv.set((article.tags || []).map(t => t.name).join(', '));
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load article:', err);
        this.error.set('Failed to load article');
        this.isLoading.set(false);
      }
    });
  }

  addBlock() {
    const t = this.newBlockType();
    const current = this.model();
    const content = [...current.content];

    switch (t) {
      case 'paragraph':
        content.push({ type: 'paragraph', text: 'New paragraph...' });
        break;
      case 'heading':
        content.push({ type: 'heading', level: 2, id: 'section-' + (content.length + 1), text: 'New section' });
        break;
      case 'richtext':
        // Create a dedicated richtext block that stores HTML produced by the editor.
        content.push({ type: 'richtext', html: '<p>New rich text content</p>' });
        break;
      case 'code':
        content.push({ type: 'code', language: 'typescript', code: 'console.log("hello");' });
        break;
      case 'list':
        content.push({ type: 'list', ordered: false, items: ['Item 1', 'Item 2'] });
        break;
      case 'callout':
        content.push({ type: 'callout', variant: 'note', heading: 'Note', text: 'Callout content' });
        break;
      case 'table':
        content.push({ type: 'table', headers: ['A', 'B'], rows: [['1', '2']] });
        break;
      case 'takeaways':
        content.push({ type: 'takeaways', items: ['Key point'] });
        break;
      case 'faq':
        content.push({ type: 'faq', items: [{ q: 'Q?', a: 'A.' }] });
        break;
      case 'terminal':
        content.push({ type: 'terminal', lines: ['command 1', 'command 2'] });
        break;
    }

    this.model.update(m => ({ ...m, content }));
  }

  moveUp(i: number) {
    if (i <= 0) return;
    this.model.update(m => {
      const content = [...m.content];
      [content[i - 1], content[i]] = [content[i], content[i - 1]];
      return { ...m, content };
    });
  }

  moveDown(i: number) {
    const current = this.model();
    if (i >= current.content.length - 1) return;
    this.model.update(m => {
      const content = [...m.content];
      [content[i + 1], content[i]] = [content[i], content[i + 1]];
      return { ...m, content };
    });
  }

  removeBlock(i: number) {
    this.model.update(m => {
      const content = [...m.content];
      content.splice(i, 1);
      return { ...m, content };
    });
  }

  private buildRequest(): any {
    const current = this.model();
    const tagsCsvTrimmed = this.tagsCsv().trim();
    
    let tagIds = current.tagIds || [];
    if (tagsCsvTrimmed) {
      // If user typed new tags manually, look them up by name
      const newTags = tagsCsvTrimmed.split(',').map(s => s.trim()).filter(s => s);
      const matchedIds = newTags.map(name => {
        const found = this.allTags().find(t => t.name.toLowerCase() === name.toLowerCase());
        return found ? found.id : null;
      }).filter(id => id !== null);
      tagIds = matchedIds as number[];
    }

    return {
      title: current.title,
      slug: current.slug,
      excerpt: current.excerpt,
      featuredImage: current.featuredImage,
      readingTime: current.readingTime,
      status: current.status || 'draft',
      publishedAt: current.publishedAt,
      authorId: current.authorId,
      categoryId: current.categoryId,
      tagIds,
      content: current.content,
    };
  }

  saveRaw() {
    const request = this.buildRequest();
    this.isSaving.set(true);
    this.error.set(undefined);

    if (this.editingId) {
      this.articleApi.updateArticle(this.editingId, request).subscribe({
        next: (response) => {
          const article = response.data;
          const model: EditorModel = {
            id: article.id,
            title: article.title,
            slug: article.slug,
            excerpt: article.excerpt,
            featuredImage: article.featuredImage,
            content: article.content || [],
            status: article.status,
            publishedAt: article.publishedAt,
            readingTime: article.readingTime,
            authorId: article.author.id,
            categoryId: article.category.id,
            tagIds: (article.tags || []).map(t => t.id),
            author: article.author,
            category: article.category,
            tags: article.tags,
          };
          this.model.set(model);
          this.isSaving.set(false);
          return this.editingId;
        },
        error: (err) => {
          console.error('Failed to update article:', err);
          this.error.set('Failed to save article');
          this.isSaving.set(false);
        }
      });
    } else {
      this.articleApi.createArticle(request).subscribe({
        next: (response) => {
          const article = response.data;
          const model: EditorModel = {
            id: article.id,
            title: article.title,
            slug: article.slug,
            excerpt: article.excerpt,
            featuredImage: article.featuredImage,
            content: article.content || [],
            status: article.status,
            publishedAt: article.publishedAt,
            readingTime: article.readingTime,
            authorId: article.author.id,
            categoryId: article.category.id,
            tagIds: (article.tags || []).map(t => t.id),
            author: article.author,
            category: article.category,
            tags: article.tags,
          };
          this.editingId = article.id;
          this.model.set(model);
          this.isSaving.set(false);
          return article.id;
        },
        error: (err) => {
          console.error('Failed to create article:', err);
          this.error.set('Failed to create article');
          this.isSaving.set(false);
        }
      });
    }
  }

  saveDraft() {
    this.model.update(m => ({ ...m, status: 'draft' }));
    this.saveRaw();
    alert('Draft saved');
  }

  publish() {
    const current = this.model();
    if (!current.title || !current.title.trim()) return alert('Title is required');
    if (!current.slug || !current.slug.trim()) return alert('Slug is required');
    if (!current.excerpt || !current.excerpt.trim()) return alert('Excerpt is required');
    if (!current.categoryId) return alert('Please select a category');
    if (!current.content || !current.content.length) return alert('Article content cannot be empty');

    this.model.update(m => ({
      ...m,
      status: 'published',
      publishedAt: m.publishedAt || new Date().toISOString().slice(0, 10),
    }));
    
    const request = this.buildRequest();
    this.isSaving.set(true);
    this.error.set(undefined);

    const operation = this.editingId
      ? this.articleApi.updateArticle(this.editingId, request)
      : this.articleApi.createArticle(request);

    operation.subscribe({
      next: (response) => {
        this.isSaving.set(false);
        alert('Published successfully');
        this.router.navigate(['/admin/articles']);
      },
      error: (err) => {
        console.error('Failed to publish article:', err);
        this.error.set('Failed to publish article');
        this.isSaving.set(false);
      }
    });
  }
}
