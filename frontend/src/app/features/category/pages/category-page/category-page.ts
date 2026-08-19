import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RoutePlaceholder } from '../../../../shared/components/route-placeholder/route-placeholder';

@Component({
  selector: 'dj-category-page',
  imports: [RoutePlaceholder],
  template: `<dj-route-placeholder
    pageTitle="Category"
    step="Step 8"
    description="Articles filtered by a single topic, reusing the blog listing card and pagination patterns."
  />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoryPage {}
