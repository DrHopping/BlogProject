import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {
  private backendUrl = `${environment.apiUrl}/articles`

  constructor(private http: HttpClient) { }

  getArticles(): Observable

}
