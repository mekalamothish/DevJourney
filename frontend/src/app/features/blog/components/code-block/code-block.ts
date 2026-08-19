import { ChangeDetectionStrategy, Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'dj-code-block',
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="my-6 rounded-md border bg-canvas">
    <div class="flex items-center justify-between px-3 py-2 text-xs text-ink/60">
      <div class="font-mono text-xs">{{ language || 'Code' }}</div>
      <button (click)="copy()" class="btn-ghost text-xs">Copy</button>
    </div>
    <pre class="overflow-auto p-4 text-sm font-mono"><code>{{ code }}</code></pre>
  </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CodeBlock {
  @Input() code = '';
  @Input() language?: string;

  async copy() {
    try {
      await navigator.clipboard.writeText(this.code);
      // minimal feedback: temporarily change button text? rely on user discovering
      // For accessibility we could announce via ARIA-live; skipping for brevity
    } catch (e) {
      console.error('Copy failed', e);
    }
  }
}
