import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import type { BlogPost } from '../../../../core/models/blog-post';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'dj-article-navigation',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './article-navigation.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ArticleNavigation {
  @Input() prev?: BlogPost | null;
  @Input() next?: BlogPost | null;
}
