import { Component, OnInit, OnDestroy } from '@angular/core';
import { BlogService } from 'src/app/_services/blog.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Blog } from 'src/app/_models/blog';
import { Subscription } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-update-blog',
  templateUrl: './update-blog.component.html',
  styleUrls: ['./update-blog.component.css']
})
export class UpdateBlogComponent implements OnInit, OnDestroy {
  private sub = new Subscription();

  blogForm: FormGroup;
  loading = false;
  submitted = false;
  loaded = false;
  blogId: number;
  error = '';

  get f() { return this.blogForm.controls; }

  constructor(private blogService: BlogService, private route: ActivatedRoute, private formBuilder: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.blogId = +this.route.snapshot.paramMap.get('id')
    this.sub = this.blogService.getBlog(this.blogId).subscribe(b => this.createForm(b));
  }

  createForm(blog: Blog) {
    this.blogForm = this.formBuilder.group({
      name: [blog.name, Validators.required],
      description: [blog.description, Validators.required]
    });
    this.loaded = true;
  }

  onSubmit() {
    this.submitted = true;

    if (this.blogForm.invalid) { return }

    this.loading = true;
    this.blogService.updateBlog(this.blogId, this.f.name.value, this.f.description.value)
      .subscribe(
        data => {
          this.router.navigateByUrl(`/home`);
        },
        error => {
          this.error = error;
          this.loading = false;
        }
      );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
