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

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    ArticleListComponent,
    BlogListComponent,
    ArticleComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: 'articles', component: ArticleListComponent },
      { path: 'blogs', component: BlogListComponent },
      { path: 'articles/:id', component: ArticleComponent },
      { path: 'login', component: LoginComponent },
    ])
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
