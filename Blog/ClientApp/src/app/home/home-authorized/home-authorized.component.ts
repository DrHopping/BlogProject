import { Component, OnInit, OnDestroy } from '@angular/core';
import { BlogService } from 'src/app/_services/blog.service';
import { Subscription } from 'rxjs';
import { Blog } from 'src/app/_models/blog';
import { CurrentUserService } from 'src/app/_services/current-user.service';
import { User } from 'src/app/_models/user';
import { AuthUser } from 'src/app/_models/authUser';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-home-authorized',
  templateUrl: './home-authorized.component.html',
  styleUrls: ['./home-authorized.component.css']
})
export class HomeAuthorizedComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  authUser: AuthUser;
  user: User;
  blogs: Blog[];

  constructor(private userService: UserService, private blogService: BlogService, private currentUserService: CurrentUserService) { }

  ngOnInit(): void {
    this.sub.add(this.currentUserService.currentUser$.subscribe(u => this.authUser = u));
    this.sub.add(this.userService.getCurrentUser().subscribe(u => this.user = u));
    this.sub.add(this.blogService.getUserBlogs(this.authUser.id).subscribe(b => this.blogs = b));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  onDelete(blogId: number) {
    this.blogService.deleteBlog(blogId).subscribe(
      data => {
        this.blogs = this.blogs.filter(b => b.id != blogId)
      });
  }
}
