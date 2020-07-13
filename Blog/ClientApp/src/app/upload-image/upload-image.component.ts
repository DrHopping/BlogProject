import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-upload-image',
  templateUrl: './upload-image.component.html',
  styleUrls: ['./upload-image.component.css']
})
export class UploadImageComponent implements OnInit {

  image: File;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  onFileSelected(event) {
    this.image = event.target.files[0];
    console.log(this.image);
  }

  upload() {
    const fd = new FormData();
    fd.append('image', this.image, this.image.name);
    this.http.post<any>('https://api.imgur.com/3/image', fd).subscribe(res => console.log(res.data.link));
  }

}
