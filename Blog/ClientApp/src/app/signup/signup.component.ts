import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';
import { first } from 'rxjs/operators';
import { CurrentUserService } from '../_services/current-user.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {
  public signupForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';

  get f() { return this.signupForm.controls; }

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthenticationService,
    private currentUserService: CurrentUserService
  ) {
    if (this.currentUserService.getCurrentUser()) {
      this.router.navigate(['/articles'])
    }
  }

  ngOnInit(): void {
    this.signupForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  OnSubmit() {
    this.submitted = true;

    if (this.signupForm.invalid) { return }

    this.loading = true;
    this.authService.signup(this.f.username.value, this.f.password.value, this.f.email.value)
      .subscribe(
        data => {
          this.authService.login(this.f.username.value, this.f.password.value)
            .subscribe(
              data => {
                this.router.navigate(['/']);
              },
              error => {
                this.error = error;
                this.loading = false;
              }
            );
        },
        error => {
          this.error = error;
          this.loading = false;
        }
      );
  }
}
