import { Component, OnInit, OnDestroy } from '@angular/core';
import { ArticleService } from '../_services/article.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Article } from '../_models/article';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();


  tags: string;
  filter: string;
  articles: Article[];

  constructor(private articleService: ArticleService, private router: Router, private route: ActivatedRoute) { }


  ngOnInit(): void {
    this.tags = this.route.snapshot.queryParamMap.get("tags");
    this.filter = this.route.snapshot.queryParamMap.get("filter");
    this.sub = this.articleService.searchArticles(this.router.url).subscribe(a => this.articles = a);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
