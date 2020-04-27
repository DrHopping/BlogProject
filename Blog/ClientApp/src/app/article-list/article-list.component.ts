import { Component, OnInit, OnDestroy } from '@angular/core';
import { Article } from '../_models/article';
import { Subscription } from 'rxjs';
import { ArticleService } from '../_services/article.service';
import { AuthenticationService } from '../_services/authentication.service';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})
export class ArticleListComponent implements OnInit, OnDestroy {
  articles: Article[];

  role = '';

  private sub: Subscription = new Subscription();

  constructor(private articleService: ArticleService, private authService: AuthenticationService) { }

  ngOnInit(): void {
    this.sub = this.articleService.getArticles().subscribe(a => this.articles = a)
    this.role = this.authService.currentUserValue.role;
  }
  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
