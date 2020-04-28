import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthUser } from '../_models/authUser';

@Injectable({
  providedIn: 'root'
})
export class CurrentUserService {
  private currentUserSubject: BehaviorSubject<AuthUser>;

  public currentUser$: Observable<AuthUser>;

  constructor() {
    this.currentUserSubject = new BehaviorSubject<AuthUser>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  getCurrentUser(): AuthUser {
    return JSON.parse(localStorage.getItem('currentUser'));
  }

  getCurrentUser$(): Observable<AuthUser> {
    return this.currentUser$;
  }
  setCurrentUser(user: AuthUser): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  removeCurrentUser(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}

