import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'dj-blog-card-skeleton',
  standalone: true,
  templateUrl: './blog-card-skeleton.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlogCardSkeleton {}
