import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private backendUrl = `${environment.apiUrl}/comments`

  constructor(private http: HttpClient) { }

  postComment(content: string, articleId: number): Observable<Comment> {
    return this.http.post<Comment>(this.backendUrl, { content, articleId });
  }

  deleteComment(id: number): Observable<Comment> {
    return this.http.delete<Comment>(`${this.backendUrl}/${id}`);
  }
}
