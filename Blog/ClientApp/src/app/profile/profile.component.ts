import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../_services/user.service';
import { Subscription } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  selectedTab: number = 1;
  user: User;
  tabs: string[] = ['Info', 'Change password', 'Upload Avatar']

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.sub = this.userService.getCurrentUser()
      .subscribe(u => this.user = u);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  changeTab(event, id: number) {
    event.preventDefault();
    this.selectedTab = id;
  }
}
