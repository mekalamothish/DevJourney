import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RoutePlaceholder } from '../../../../shared/components/route-placeholder/route-placeholder';

@Component({
  selector: 'dj-resources-page',
  imports: [RoutePlaceholder],
  template: `<dj-route-placeholder pageTitle="Resources" step="Step 4" description="Resources listing placeholder — full content ships in a later step." />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResourcesPage {}
