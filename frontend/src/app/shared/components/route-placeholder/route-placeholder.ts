import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * Renders in place of a page that the routing skeleton already wires up but
 * whose real content ships in a later step of the build plan (Section 37).
 * Swap the loadComponent target in app.routes.ts for the real page component
 * when that step is built — nothing else needs to change.
 */
@Component({
  selector: 'dj-route-placeholder',
  templateUrl: './route-placeholder.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoutePlaceholder {
  readonly pageTitle = input.required<string>();
  readonly step = input.required<string>();
  readonly description = input.required<string>();
}
