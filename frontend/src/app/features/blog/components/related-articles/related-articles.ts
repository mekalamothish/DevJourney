import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import type { BlogPost } from '../../../../core/models/blog-post';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BlogCard } from '../../../../shared/components/blog-card/blog-card';

@Component({
  selector: 'dj-related-articles',
  standalone: true,
  imports: [CommonModule, BlogCard,RouterLink],
  templateUrl: './related-articles.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RelatedArticles {
  @Input() posts: BlogPost[] = [];
  @Input() compact?: boolean;
}
