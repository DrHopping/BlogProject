import { Component, OnInit, OnDestroy, AfterViewInit, AfterViewChecked } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ArticleService } from '../_services/article.service';
import { Article } from '../_models/article';
import { AuthUser } from '../_models/authUser';
import { Comment } from '../_models/comment';
import { CurrentUserService } from '../_services/current-user.service';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.css']
})
export class ArticleComponent implements OnInit, OnDestroy, AfterViewChecked {
  private sub: Subscription = new Subscription();

  scrolled = false;
  article: Article;
  user: AuthUser;

  constructor(private route: ActivatedRoute, private articleService: ArticleService, private currentUserService: CurrentUserService) { }

  ngOnInit() {
    this.sub.add(this.articleService.getArticle(+this.route.snapshot.paramMap.get('id')).subscribe(a => this.article = a))
    this.sub.add(this.currentUserService.currentUser$.subscribe(u => this.user = u));
  }

  onCommentPost(comment: Comment) {
    this.article.comments.push(comment);
    this.scrolled = true;
  }

  onCommentDelete(id: number) {
    this.article.comments = this.article.comments.filter(c => c.id != id)
  }

  ngAfterViewChecked(): void {
    if (this.scrolled) {
      window.scroll(0, document.body.scrollHeight);
      console.log(1234);
      this.scrolled = false;
    }
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
