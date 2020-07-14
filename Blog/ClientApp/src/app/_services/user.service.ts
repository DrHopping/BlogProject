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

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/users`);
  }

  getModerators(): Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/users/moderators`);
  }

  getRegularUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/users/regular`);
  }

  promoteUser(id: number): Observable<any> {
    return this.http.put<any>(`${environment.apiUrl}/users/promote`, { id });
  }

  unpromoteUser(id: number): Observable<any> {
    return this.http.put<any>(`${environment.apiUrl}/users/unpromote`, { id });
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete<any>(`${environment.apiUrl}/users/${id}`);
  }

  updateUser(username, avatarUrl, email, info): Observable<any> {
    return this.http.put<any>(`${environment.apiUrl}/users/${this.currentUserService.getCurrentUser().id}`, { username, avatarUrl, email, info })
      .pipe(map(() => {
        let user = this.currentUserService.getCurrentUser();
        if (username != null) user.username = username;
        this.currentUserService.setCurrentUser(user);
      }));
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/users/${id}`);
  }

  getPublicUserInfo(id: number): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/users/${id}/public`);
  }
  changePassword(oldPassword, newPassword): Observable<any> {
    return this.http.put<any>(`${environment.apiUrl}/users/${this.currentUserService.getCurrentUser().id}/password`, { oldPassword, newPassword })
  }
}