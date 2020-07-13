import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { ArticleListComponent } from './article-list/article-list.component';
import { BlogListComponent } from './blog-list/blog-list.component';
import { ArticleComponent } from './article/article.component';
import { LoginComponent } from './login/login.component';
import { JwtInterceptor } from './_helpers/jwt.interceptor';
import { ErrorInterceptor } from './_helpers/error.interceptor';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ProfileComponent } from './profile/profile.component';
import { SignupComponent } from './signup/signup.component';
import { ProfileSettingsComponent } from './profile/profile-settings/profile-settings.component';
import { ProfilePasswordComponent } from './profile/profile-password/profile-password.component';
import { AuthGuard } from './_helpers/auth.guard';
import { HomeGetstartedComponent } from './home/home-getstarted/home-getstarted.component';
import { HomeAuthorizedComponent } from './home/home-authorized/home-authorized.component';
import { HomeComponent } from './home/home.component';
import { CreateBlogComponent } from './create/create-blog/create-blog.component';
import { BlogComponent } from './blog/blog.component';
import { PublicProfileComponent } from './public-profile/public-profile.component';
import { ArticlesMainComponent } from './articles-main/articles-main.component';
import { CreateArticleComponent } from './create/create-article/create-article.component';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { UploadImageComponent } from './upload-image/upload-image.component';
import { ImgurInterceptor } from './_helpers/imgur.interceptor';
import { ProfileAvatarComponent } from './profile/profile-avatar/profile-avatar.component';
import { UpdateBlogComponent } from './update/update-blog/update-blog.component';
import { UpdateArticleComponent } from './update/update-article/update-article.component';
import { SearchComponent } from './search/search.component';
import { SearchWidgetComponent } from './widgets/search-widget/search-widget.component';
import { AdminPageComponent } from './admin-page/admin-page.component';
import { AdminGuard } from './_helpers/admin.guard';
import { CommentListComponent } from './comment/comment-list/comment-list.component';
import { CommentFormComponent } from './comment/comment-form/comment-form.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    ArticleListComponent,
    BlogListComponent,
    ArticleComponent,
    LoginComponent,
    ProfileComponent,
    SignupComponent,
    ProfileSettingsComponent,
    ProfilePasswordComponent,
    HomeGetstartedComponent,
    HomeAuthorizedComponent,
    HomeComponent,
    CreateBlogComponent,
    CreateArticleComponent,
    BlogComponent,
    PublicProfileComponent,
    ArticlesMainComponent,
    UploadImageComponent,
    ProfileAvatarComponent,
    UpdateBlogComponent,
    UpdateArticleComponent,
    SearchComponent,
    SearchWidgetComponent,
    AdminPageComponent,
    CommentListComponent,
    CommentFormComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    NgbModule,
    AngularEditorModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'articles', component: ArticlesMainComponent },
      { path: 'blogs', component: BlogListComponent },
      { path: 'blogs/:id', component: BlogComponent },
      { path: 'articles/:id', component: ArticleComponent },
      { path: 'login', component: LoginComponent },
      { path: 'signup', component: SignupComponent },
      { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
      { path: 'home', component: HomeComponent },
      { path: 'create/blog', component: CreateBlogComponent, canActivate: [AuthGuard] },
      { path: 'users/:id', component: PublicProfileComponent },
      { path: 'create/article', component: CreateArticleComponent, canActivate: [AuthGuard] },
      { path: 'blogs/:id/update', component: UpdateBlogComponent, canActivate: [AuthGuard] },
      { path: 'articles/:id/update', component: UpdateArticleComponent, canActivate: [AuthGuard] },
      { path: 'search', component: SearchComponent },
      { path: 'admin', component: AdminPageComponent, canActivate: [AdminGuard] },
    ], { onSameUrlNavigation: 'reload' })
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ImgurInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
