import { Injectable } from '@angular/core';
import { AuthUser } from '../_models/authUser';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, BehaviorSubject } from 'rxjs';
import { User } from '../_models/user';
import { map } from 'rxjs/operators';
import { CurrentUserService } from './current-user.service';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient, private currentUserService: CurrentUserService) {
  }

  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/users/${this.currentUserService.getCurrentUser().id}`);
  }

  updateUser(username, avatarUrl, email, info): Observable<any> {
    return this.http.put<any>(`${environment.apiUrl}/users/${this.currentUserService.getCurrentUser().id}`, { username, avatarUrl, email, info })
      .pipe(map(() => {
        let user = this.currentUserService.getCurrentUser();
        user.username = username;
        this.currentUserService.setCurrentUser(user);
      }));
  }

  changePassword(oldPassword, newPassword): Observable<any> {
    return this.http.put<any>(`${environment.apiUrl}/users/${this.currentUserService.getCurrentUser().id}/password`, { oldPassword, newPassword })
  }
}