import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthUser } from 'src/app/_models/authUser';
import { Blog } from 'src/app/_models/blog';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Tag } from 'src/app/_models/tag';
import { CurrentUserService } from 'src/app/_services/current-user.service';
import { BlogService } from 'src/app/_services/blog.service';
import { ArticleService } from 'src/app/_services/article.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { Subscription } from 'rxjs';
import { Article } from 'src/app/_models/article';


@Component({
  selector: 'app-update-article',
  templateUrl: './update-article.component.html',
  styleUrls: ['./update-article.component.css']
})
export class UpdateArticleComponent implements OnInit, OnDestroy {
  private sub = new Subscription();

  article: Article;
  articleForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  articleId: number;

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


  get f() { return this.articleForm.controls; }

  constructor(
    private articleService: ArticleService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.articleId = +this.route.snapshot.paramMap.get('id');
    this.sub.add(this.articleService.getArticle(this.articleId).subscribe(a => { this.article = a; this.createForm(a) }));
  }

  createForm(article: Article) {
    const tags = article.tags.reduce((result, curr) => result + curr.name + ",", "").replace(/,\s*$/, "");

    this.articleForm = this.formBuilder.group({
      title: [article.title, Validators.required],
      tags: [tags, Validators.required],
      imageUrl: [article.imageUrl, Validators.required],
      content: [article.content, Validators.required],
    });
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
    this.articleService.updateArticle(this.articleId, this.f.title.value, tags, this.f.content.value, this.f.imageUrl.value)
      .subscribe(
        data => {
          this.router.navigateByUrl(`articles/${this.articleId}`);
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
