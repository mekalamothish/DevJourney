import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import type { Author } from '../../../../core/models/blog-post';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'dj-article-author',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './article-author.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ArticleAuthor {
  @Input() author!: Author;
  @Input() inline?: boolean;
}
