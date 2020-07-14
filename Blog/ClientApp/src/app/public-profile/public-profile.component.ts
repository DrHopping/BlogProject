import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthUser } from '../_models/authUser';
import { User } from '../_models/user';
import { Blog } from '../_models/blog';
import { UserService } from '../_services/user.service';
import { BlogService } from '../_services/blog.service';
import { CurrentUserService } from '../_services/current-user.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-public-profile',
  templateUrl: './public-profile.component.html',
  styleUrls: ['./public-profile.component.css']
})
export class PublicProfileComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  user: User;
  blogs: Blog[];

  constructor(private route: ActivatedRoute, private userService: UserService, private blogService: BlogService) { }

  ngOnInit(): void {
    const userId = +this.route.snapshot.paramMap.get('id');
    this.sub.add(this.userService.getPublicUserInfo(userId).subscribe(u => this.user = u));
    this.sub.add(this.blogService.getUserBlogs(userId).subscribe(bs => this.blogs = bs));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
