import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import type { ArticleBlock } from '../../../../core/models/blog-post';
import { CommonModule } from '@angular/common';
import { CodeBlock } from '../code-block/code-block';
import { ArticleFaq } from '../faq/faq';

@Component({
  selector: 'dj-article-content',
  standalone: true,
  imports: [CommonModule, CodeBlock, ArticleFaq],
  templateUrl: './article-content.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ArticleContent {
  @Input() blocks: ArticleBlock[] = [];
}
