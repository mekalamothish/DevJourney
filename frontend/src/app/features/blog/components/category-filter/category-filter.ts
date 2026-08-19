import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'dj-category-filter',
  templateUrl: './category-filter.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoryFilter {
  @Input() categories: string[] = [];
  @Input() selected?: string;
  @Output() selectedChange = new EventEmitter<string | undefined>();

  select(cat?: string) {
    this.selectedChange.emit(cat);
  }
}
