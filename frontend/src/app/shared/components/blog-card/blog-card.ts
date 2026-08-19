import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import type { BlogPost } from '../../../core/models/blog-post';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'dj-blog-card',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './blog-card.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlogCard {
  @Input() post!: BlogPost;
}
