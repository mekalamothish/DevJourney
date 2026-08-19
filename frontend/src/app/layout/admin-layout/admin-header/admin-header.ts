import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SITE_CONFIG } from '../../../core/constants/site-config';

@Component({
  selector: 'dj-admin-header',
  imports: [RouterLink],
  templateUrl: './admin-header.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminHeader {
  protected readonly site = SITE_CONFIG;
  @Input() onToggle?: () => void;
}
