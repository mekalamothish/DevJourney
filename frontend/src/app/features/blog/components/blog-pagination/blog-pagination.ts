import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'dj-blog-pagination',
  templateUrl: './blog-pagination.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlogPagination {
  @Input() current = 1;
  @Input() total = 1;
  @Output() pageChange = new EventEmitter<number>();

  get pages(): number[] {
    return Array.from({ length: Math.max(0, this.total) }, (_, i) => i + 1);
  }

  go(page: number) {
    if (page < 1 || page > this.total || page === this.current) return;
    this.pageChange.emit(page);
  }
}
