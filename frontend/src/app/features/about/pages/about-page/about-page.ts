import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RoutePlaceholder } from '../../../../shared/components/route-placeholder/route-placeholder';

@Component({
  selector: 'dj-about-page',
  imports: [RoutePlaceholder],
  template: `<dj-route-placeholder
    pageTitle="About"
    step="Step 5"
    description="Author bio and background as a dedicated page. The home page shows a condensed teaser that links here."
  />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AboutPage {}
