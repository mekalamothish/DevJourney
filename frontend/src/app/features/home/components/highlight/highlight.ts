import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'dj-highlight',
  imports: [RouterLink],
  templateUrl: './highlight.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Highlight {}
