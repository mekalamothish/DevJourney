import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import type { BlogPost } from '../../../../core/models/blog-post';
import { CommonModule } from '@angular/common';
import { ArticleAuthor } from '../article-author/article-author';

@Component({
  selector: 'dj-article-header',
  standalone: true,
  imports: [CommonModule, ArticleAuthor],
  templateUrl: './article-header.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ArticleHeader {
  @Input() post!: BlogPost;
}
