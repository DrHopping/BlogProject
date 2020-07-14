import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Tag } from '../_models/tag';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TagService {

  constructor(private http: HttpClient) { }

  getAllTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${environment.apiUrl}/tags`);
  }

  getTagsById(id: number): Observable<Tag> {
    return this.http.get<Tag>(`${environment.apiUrl}/tags/${id}`);
  }

  getTopTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${environment.apiUrl}/tags/top`);
  }
}
