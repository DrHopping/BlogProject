import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Article } from '../_models/article';
import { Observable } from 'rxjs';
import { Tag } from '../_models/tag';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {
  private backendUrl = `${environment.apiUrl}/articles`

  constructor(private http: HttpClient) { }

  getArticles(): Observable<Article[]> {
    return this.http.get<Article[]>(this.backendUrl);
  }

  getArticle(id: number): Observable<Article> {
    return this.http.get<Article>(`${this.backendUrl}/${id}`);
  }

  createArticle(blogId: number, title: string, tags: Tag[], content: string, imageUrl: string): Observable<Article> {
    return this.http.post<Article>(this.backendUrl, { blogId, title, tags, content, imageUrl });
  }

  updateArticle(articleId: number, title: string, tags: Tag[], content: string, imageUrl: string): Observable<any> {
    return this.http.put<any>(`${this.backendUrl}/${articleId}`, { title, tags, content, imageUrl })
  }

  deleteArticle(articleId: number): Observable<any> {
    return this.http.delete<any>(`${this.backendUrl}/${articleId}`);
  }

  searchArticles(url: string): Observable<Article[]> {
    return this.http.get<any>(`${environment.apiUrl}${url}`)
  }
}
