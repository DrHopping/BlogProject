import { Component, OnInit, OnDestroy } from '@angular/core';
import { Article } from '../_models/article';
import { Subscription } from 'rxjs';
import { ArticleService } from '../_services/article.service';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})
export class ArticleListComponent implements OnInit, OnDestroy {
  private articles: Article[];
  private sub: Subscription = new Subscription();

  constructor(private articleService: ArticleService) { }

  ngOnInit(): void {
    this.sub = this.articleService.getArticles().subscribe(a => this.articles = a)
  }
  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
