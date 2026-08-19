import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'dj-author',
  templateUrl: './author.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Author {}
