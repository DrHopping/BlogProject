import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Blog } from '../_models/blog';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BlogService {
  private backendUrl = `${environment.apiUrl}/blogs`

  constructor(private http: HttpClient) { }

  getBlog(id: number): Observable<Blog> {
    return this.http.get<Blog>(`${this.backendUrl}/${id}`);
  }

  getBlogs(): Observable<Blog[]> {
    return this.http.get<Blog[]>(this.backendUrl);
  }

  createBlog(name, description): Observable<Blog> {
    return this.http.post<Blog>(this.backendUrl, { name, description });
  }

  getUserBlogs(userId: number): Observable<Blog[]> {
    return this.http.get<Blog[]>(`${environment.apiUrl}/users/${userId}/blogs`);
  }

  deleteBlog(blogId: number): Observable<any> {
    return this.http.delete<any>(`${this.backendUrl}/${blogId}`);
  }

  updateBlog(blogId: number, name: string, description: string): Observable<any> {
    return this.http.put<any>(`${this.backendUrl}/${blogId}`, { name, description })
  }

}
