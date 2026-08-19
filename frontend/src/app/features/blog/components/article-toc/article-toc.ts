import { ChangeDetectionStrategy, Component, Input, HostListener } from '@angular/core';
import type { ArticleBlock } from '../../../../core/models/blog-post';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'dj-article-toc',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './article-toc.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ArticleToc {
  @Input() blocks: ArticleBlock[] = [];

  collapsed = true;

  get headings() {
    return this.blocks.filter(b => b.type === 'heading' || b.type === 'subheading') as any[];
  }

  goTo(id: string) {
    // use native scroll with offset to account for sticky header
    const el = document.getElementById(id);
    if (!el) return;
    const headerOffset = 80;
    const y = el.getBoundingClientRect().top + window.scrollY - headerOffset;
    window.scrollTo({ top: y, behavior: 'smooth' });
    this.collapsed = true;
  }
}
