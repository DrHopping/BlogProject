import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ImageUploadService } from 'src/app/_services/image-upload.service';
import { UserService } from 'src/app/_services/user.service';
import { pipe } from 'rxjs';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-profile-avatar',
  templateUrl: './profile-avatar.component.html',
  styleUrls: ['./profile-avatar.component.css']
})
export class ProfileAvatarComponent implements OnInit {

  fileName = 'Choose file';
  loading = false;
  submitted = false;
  success = false;
  error = '';
  avatarUrl: string;


  @Input() user: User;

  constructor(private imageUploadService: ImageUploadService, private userService: UserService) { }

  ngOnInit(): void {
    this.avatarUrl = this.user.avatarUrl;
  }

  onFileSelected(event) {
    const image = event.target.files[0];
    this.fileName = image.name;
    this.imageUploadService.uploadImage(image).subscribe(
      data => {
        this.avatarUrl = data;
      }
    );
  }

  OnSubmit() {
    this.submitted = true;

    this.loading = true;
    this.userService.updateUser(null, this.avatarUrl, null, null)
      .subscribe(
        data => {
          this.success = true;
          this.loading = false;
        },
        error => {
          this.error = error;
          this.loading = false;
        }
      );
  }

  removeAlert(event) {
    event.preventDefault();
    this.success = false;
    this.error = '';
  }
}
