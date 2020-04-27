import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ArticleService } from '../_services/article.service';
import { Article } from '../_models/article';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.css']
})
export class ArticleComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  articleItem: Article;

  constructor(private router: ActivatedRoute, private _postService: ArticleService) { }

  ngOnInit() {
    this.sub.add(this._postService.getArticle(+this.router.snapshot.paramMap.get('id')).subscribe(a => this.articleItem = a))
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
