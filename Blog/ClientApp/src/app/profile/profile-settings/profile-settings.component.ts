import { Component, OnInit, Input } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { User } from 'src/app/_models/user';
import { Subscription } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from 'src/app/_services/authentication.service';

@Component({
  selector: 'app-profile-settings',
  templateUrl: './profile-settings.component.html',
  styleUrls: ['./profile-settings.component.css']
})
export class ProfileSettingsComponent implements OnInit {
  userUpdateForm: FormGroup;
  loading = false;
  submitted = false;
  success = false;
  error = '';

  @Input() user: User;

  get f() { return this.userUpdateForm.controls; }

  constructor(private userService: UserService, private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.userUpdateForm = this.formBuilder.group({
      username: [this.user.username, Validators.required],
      avatarUrl: [this.user.avatarUrl, Validators.required],
      email: [this.user.email, [Validators.email, Validators.required]],
      info: [this.user.info, Validators.required]
    });
  }

  OnSubmit() {
    this.submitted = true;

    if (this.userUpdateForm.invalid) { return }

    this.loading = true;
    this.userService.updateUser(this.f.username.value, this.f.avatarUrl.value, this.f.email.value, this.f.info.value)
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
