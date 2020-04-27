import { Component, OnInit } from '@angular/core';
import { Blog } from '../_models/blog';
import { Subscription } from 'rxjs';
import { BlogService } from '../_services/blog.service';

@Component({
  selector: 'app-blog-list',
  templateUrl: './blog-list.component.html',
  styleUrls: ['./blog-list.component.css']
})
export class BlogListComponent implements OnInit {

  blogs: Blog[];

  private sub: Subscription = new Subscription();

  constructor(private blogService: BlogService) { }

  ngOnInit(): void {
    this.sub = this.blogService.getBlogs().subscribe(b => this.blogs = b)
  }
  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

}
