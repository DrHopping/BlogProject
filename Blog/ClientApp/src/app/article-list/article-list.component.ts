import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Article } from '../_models/article';
import { Subscription } from 'rxjs';
import { ArticleService } from '../_services/article.service';
import { AuthUser } from '../_models/authUser';
import { CurrentUserService } from '../_services/current-user.service';

@Component({
  selector: 'app-article-list',
  templateUrl: './article-list.component.html',
  styleUrls: ['./article-list.component.css']
})
export class ArticleListComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  @Input() articles: Article[];

  user: AuthUser;
  page = 1;
  pageSize = 2;

  constructor(private articleService: ArticleService, private currentUserService: CurrentUserService) { }

  get sortedArticles() {
    var sorted = this.articles.sort((a, b) => {
      return new Date(b.lastUpdated).getTime() - new Date(a.lastUpdated).getTime();
    })
    return sorted;
  }

  get pageArticles(): Article[] {
    return this.sortedArticles
      .map((article, i) => ({ id: i + 1, ...article }))
      .slice((this.page - 1) * this.pageSize, (this.page - 1) * this.pageSize + this.pageSize);
  }

  older(e) { this.page++; e.preventDefault() }
  newer(e) { this.page--; e.preventDefault() }

  ngOnInit(): void {
    this.sub = this.currentUserService.currentUser$.subscribe(u => this.user = u);
  }

  onDelete(articleId: number) {
    this.articleService.deleteArticle(articleId).subscribe(
      data => {
        this.articles = this.articles.filter(a => a.id != articleId)
      });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }


}
