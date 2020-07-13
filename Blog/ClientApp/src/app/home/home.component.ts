import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthUser } from '../_models/authUser';
import { CurrentUserService } from '../_services/current-user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  user: AuthUser;

  constructor(private currentUserService: CurrentUserService) { }

  ngOnInit(): void {
    this.sub = this.currentUserService.currentUser$.subscribe(u => this.user = u);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
