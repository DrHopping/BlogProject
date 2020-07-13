import { Component, OnInit } from '@angular/core';
import { Blog } from '../_models/blog';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ArticleService } from '../_services/article.service';
import { BlogService } from '../_services/blog.service';
import { CurrentUserService } from '../_services/current-user.service';
import { AuthUser } from '../_models/authUser';

@Component({
  selector: 'app-blog',
  templateUrl: './blog.component.html',
  styleUrls: ['./blog.component.css']
})
export class BlogComponent implements OnInit {
  private sub: Subscription = new Subscription();

  user: AuthUser;
  blog: Blog;

  constructor(private router: ActivatedRoute, private blogService: BlogService, private currentUserService: CurrentUserService) { }

  ngOnInit() {
    this.sub.add(this.blogService.getBlog(+this.router.snapshot.paramMap.get('id')).subscribe(b => { this.blog = b }))
    this.sub.add(this.currentUserService.currentUser$.subscribe(u => this.user = u));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
