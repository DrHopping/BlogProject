import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthenticationService } from '../_services/authentication.service';
import { Subscription } from 'rxjs';
import { AuthUser } from '../_models/authUser';
import { CurrentUserService } from '../_services/current-user.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  isExpanded = false;
  user: AuthUser;

  constructor(private authService: AuthenticationService, private currentUserService: CurrentUserService) { }


  ngOnInit(): void {
    this.sub = this.currentUserService.currentUser$.subscribe(u => this.user = u);
  }

  logout() {
    this.authService.logout();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
