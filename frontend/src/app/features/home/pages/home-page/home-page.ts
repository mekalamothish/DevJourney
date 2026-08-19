import { ChangeDetectionStrategy, Component } from '@angular/core';
import { Hero } from '../../components/hero/hero';
import { Highlight } from '../../components/highlight/highlight';
import { FeaturedArticle } from '../../components/featured-article/featured-article';
import { LatestArticles } from '../../components/latest-articles/latest-articles';
import { PopularArticles } from '../../components/popular-articles/popular-articles';
import { Topics } from '../../components/topics/topics';
import { LearningRoadmap } from '../../components/learning-roadmap/learning-roadmap';
import { InterviewPrep } from '../../components/interview-prep/interview-prep';
import { Resources } from '../../components/resources/resources';
import { Author } from '../../components/author/author';
import { FinalCta } from '../../components/final-cta/final-cta';

@Component({
  selector: 'dj-home-page',
  imports: [
    Hero,
    Highlight,
    FeaturedArticle,
    LatestArticles,
    PopularArticles,
    Topics,
    LearningRoadmap,
    InterviewPrep,
    Resources,
    Author,
    FinalCta,
  ],
  templateUrl: './home-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage {}
