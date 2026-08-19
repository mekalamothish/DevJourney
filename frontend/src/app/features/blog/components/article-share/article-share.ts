import { ChangeDetectionStrategy, Component } from '@angular/core';

import { CommonModule } from '@angular/common';

@Component({
  selector: 'dj-article-share',
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="rounded-md border bg-canvas border-gray-500/50p-3">
    <div class="text-sm font-semibold text-ink mb-2">Share</div>
    <div class="flex gap-2">
      <button (click)="copyLink()" class="btn-ghost">Copy Link</button>
      <button class="btn-ghost" disabled>LinkedIn</button>
      <button class="btn-ghost" disabled>X</button>
    </div>
    <div *ngIf="copied" class="mt-2 text-sm text-green-600">Link copied</div>
  </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ArticleShare {
  copied = false;

  async copyLink() {
    try {
      await navigator.clipboard.writeText(location.href);
      this.copied = true;
      setTimeout(() => (this.copied = false), 2000);
    } catch (e) {
      console.error('Copy failed', e);
    }
  }
}
