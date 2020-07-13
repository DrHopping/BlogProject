import { Component, OnInit, OnDestroy } from '@angular/core';
import { CurrentUserService } from 'src/app/_services/current-user.service';
import { BlogService } from 'src/app/_services/blog.service';
import { ArticleService } from 'src/app/_services/article.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AuthUser } from 'src/app/_models/authUser';
import { Blog } from 'src/app/_models/blog';
import { Router, ActivatedRoute } from '@angular/router';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { Tag } from 'src/app/_models/tag';

@Component({
  selector: 'app-create-article',
  templateUrl: './create-article.component.html',
  styleUrls: ['./create-article.component.css']
})
export class CreateArticleComponent implements OnInit, OnDestroy {
  private sub = new Subscription();

  editorConfig: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'auto',
    minHeight: '300px',
    maxHeight: 'auto',
    width: 'auto',
    minWidth: '0',
    translate: 'yes',
    enableToolbar: true,
    showToolbar: true,
    placeholder: 'Enter text here...',
    defaultParagraphSeparator: '',
    defaultFontName: '',
    defaultFontSize: '',
    fonts: [
      { class: 'arial', name: 'Arial' },
      { class: 'times-new-roman', name: 'Times New Roman' },
      { class: 'calibri', name: 'Calibri' },
      { class: 'comic-sans-ms', name: 'Comic Sans MS' }
    ],
    uploadUrl: 'v1/image',
    uploadWithCredentials: false,
    sanitize: true,
    toolbarPosition: 'top',
    toolbarHiddenButtons: [
      ['insertImage',
        'insertVideo',
        'customClasses',
        'link',
        'unlink',]
    ]
  };

  user: AuthUser;
  blogs: Blog[];
  articleForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  blogId: number;


  get f() { return this.articleForm.controls; }


  constructor(
    private currentUserService: CurrentUserService,
    private blogService: BlogService,
    private articleService: ArticleService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.blogId = +this.route.snapshot.queryParams['blogId'];
    this.articleForm = this.formBuilder.group({
      title: ['', Validators.required],
      tags: ['', Validators.required],
      imageUrl: ['', Validators.required],
      content: ['', Validators.required],
      blogId: ['', Validators.required]
    });
    this.sub.add(this.currentUserService.currentUser$.subscribe(u => this.user = u));
    this.sub.add(this.blogService.getUserBlogs(this.user.id).subscribe(b => this.blogs = b));
  }

  OnSubmit() {
    this.submitted = true;

    if (this.articleForm.invalid) { return }
    var tagsString: string = this.f.tags.value;
    var tags = tagsString.split(',').map(t => {
      var tag = new Tag();
      tag.name = t;
      return tag
    })

    this.loading = true;
    this.articleService.createArticle(this.f.blogId.value, this.f.title.value, tags, this.f.content.value, this.f.imageUrl.value)
      .subscribe(
        data => {
          this.router.navigateByUrl(`articles/${data.id}`);
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
