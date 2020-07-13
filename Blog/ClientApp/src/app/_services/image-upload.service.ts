import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ImageUploadService {

  constructor(private http: HttpClient) { }

  uploadImage(image: File): Observable<string> {
    const fd = new FormData();
    fd.append('image', image, image.name);
    return this.http.post<any>(environment.imgurApiUrl, fd).pipe(map(res => { return res.data.link }));
  }

}
