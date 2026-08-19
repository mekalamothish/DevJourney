import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RoutePlaceholder } from '../../../../shared/components/route-placeholder/route-placeholder';

@Component({
  selector: 'dj-search-page',
  imports: [RoutePlaceholder],
  template: `<dj-route-placeholder
    pageTitle="Search"
    step="Step 8"
    description="Full text search across articles with live results."
  />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SearchPage {}
