import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import type { FaqItem } from '../../../../core/models/blog-post';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'dj-article-faq',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './faq.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ArticleFaq {
  @Input() items: FaqItem[] = [];

  openIndex = -1;

  toggle(i: number) {
    this.openIndex = this.openIndex === i ? -1 : i;
  }
}
