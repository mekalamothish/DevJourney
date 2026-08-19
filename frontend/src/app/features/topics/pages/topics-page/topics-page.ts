import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RoutePlaceholder } from '../../../../shared/components/route-placeholder/route-placeholder';

@Component({
  selector: 'dj-topics-page',
  imports: [RoutePlaceholder],
  template: `<dj-route-placeholder
    pageTitle="Topics"
    step="Step 8"
    description="The full topic index, every category with an article count, for browsing by subject."
  />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopicsPage {}
