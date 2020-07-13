import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BlogService } from 'src/app/_services/blog.service';
import { first } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-blog',
  templateUrl: './create-blog.component.html',
  styleUrls: ['./create-blog.component.css']
})
export class CreateBlogComponent implements OnInit {

  createBlogForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  get f() { return this.createBlogForm.controls; }

  constructor(private formBuilder: FormBuilder, private blogService: BlogService, private router: Router) { }

  ngOnInit(): void {
    this.createBlogForm = this.formBuilder.group({
      name: ['', Validators.required],
      description: ['', Validators.required]
    });
  }

  OnSubmit() {
    this.submitted = true;

    if (this.createBlogForm.invalid) { return }

    this.loading = true;
    this.blogService.createBlog(this.f.name.value, this.f.description.value)
      .subscribe(
        data => {
          this.router.navigateByUrl(`create/article?blogId=${data.id}`);
        },
        error => {
          this.error = error;
          this.loading = false;
        }
      );
  }


}
