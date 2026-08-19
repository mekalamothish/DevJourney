import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TOPICS } from '../../constants/learning.constants';

@Component({
  selector: 'dj-topics',
  imports: [RouterLink],
  templateUrl: './topics.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Topics {
  protected readonly topics = TOPICS;
}
