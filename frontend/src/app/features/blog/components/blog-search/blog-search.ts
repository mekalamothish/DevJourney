import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'dj-blog-search',
  templateUrl: './blog-search.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlogSearch {
  @Input() value = '';
  @Output() valueChange = new EventEmitter<string>();

  onInput(ev: Event) {
    const v = (ev.target as HTMLInputElement).value;
    this.valueChange.emit(v);
  }
  clear() {
  this.valueChange.emit('');
}
}
