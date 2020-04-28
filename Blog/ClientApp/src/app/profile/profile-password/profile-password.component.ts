import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-profile-password',
  templateUrl: './profile-password.component.html',
  styleUrls: ['./profile-password.component.css']
})
export class ProfilePasswordComponent implements OnInit {
  passwordUpdateForm: FormGroup;
  loading = false;
  submitted = false;
  success = false;
  error = '';

  get f() { return this.passwordUpdateForm.controls; }

  constructor(private userService: UserService, private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.passwordUpdateForm = this.formBuilder.group({
      oldPass: ['', Validators.required],
      newPass: ['', Validators.required],
    });
  }

  OnSubmit() {
    this.submitted = true;

    if (this.passwordUpdateForm.invalid) { return }

    this.loading = true;
    this.userService.changePassword(this.f.oldPass.value, this.f.newPass.value)
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
