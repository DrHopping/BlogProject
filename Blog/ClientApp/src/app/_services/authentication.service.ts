import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { map } from 'rxjs/operators';
import { AuthUser } from '../_models/authUser';
import { HttpClient } from '@angular/common/http';
import { CurrentUserService } from './current-user.service';


@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  constructor(private http: HttpClient, private currentUserService: CurrentUserService) {
  }


  login(username: string, password: string) {
    return this.http.post<AuthUser>(`${environment.apiUrl}/auth`, { username, password })
      .pipe(map(user => {
        this.currentUserService.setCurrentUser(user);
        return user;
      }));
  }

  signup(username: string, password: string, email: string) {
    return this.http.post<any>(`${environment.apiUrl}/users`, { username, password, email });
  }

  logout() {
    this.currentUserService.removeCurrentUser();
  }
}
