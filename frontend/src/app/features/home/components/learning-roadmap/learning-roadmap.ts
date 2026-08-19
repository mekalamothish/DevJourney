import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ROADMAP } from '../../constants/learning.constants';

@Component({
  selector: 'dj-learning-roadmap',
  templateUrl: './learning-roadmap.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LearningRoadmap {
  protected readonly items = ROADMAP;
}
