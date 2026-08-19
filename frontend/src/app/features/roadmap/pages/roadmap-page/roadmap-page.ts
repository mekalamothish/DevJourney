import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RoutePlaceholder } from '../../../../shared/components/route-placeholder/route-placeholder';

@Component({
  selector: 'dj-roadmap-page',
  imports: [RoutePlaceholder],
  template: `<dj-route-placeholder
    pageTitle="Learning Roadmap"
    step="Step 5"
    description="The full sequential roadmap as its own linkable page. The home page shows a condensed version of the same data."
  />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoadmapPage {}
