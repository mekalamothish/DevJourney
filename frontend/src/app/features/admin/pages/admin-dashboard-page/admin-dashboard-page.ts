import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RoutePlaceholder } from '../../../../shared/components/route-placeholder/route-placeholder';

@Component({
  selector: 'dj-admin-dashboard-page',
  imports: [RoutePlaceholder],
  template: `<dj-route-placeholder
    pageTitle="Admin"
    step="Step 9"
    description="Admin shell entry point, once the sidebar and header admin layout is built."
  />`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDashboardPage {}
