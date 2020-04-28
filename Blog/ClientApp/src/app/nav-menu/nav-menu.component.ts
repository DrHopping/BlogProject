import { Component, OnInit, OnDestroy, OnChanges, SimpleChanges } from '@angular/core';
import { AuthenticationService } from '../_services/authentication.service';
import { Observable, Subscriber, Subscription } from 'rxjs';
import { AuthUser } from '../_models/authUser';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  isExpanded = false;
  user: AuthUser;

  constructor(private authService: AuthenticationService) { }


  ngOnInit(): void {
    this.sub = this.authService.currentUser$.subscribe(u => this.user = u);
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
