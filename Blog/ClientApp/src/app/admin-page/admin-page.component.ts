import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../_services/user.service';
import { Subscription, Observable } from 'rxjs';
import { User } from '../_models/user';
import { FormControl } from '@angular/forms';
import { map, takeUntil, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.css']
})
export class AdminPageComponent implements OnInit, OnDestroy {
  private sub: Subscription = new Subscription();

  users: User[];
  users$: Observable<User[]>;
  filter = new FormControl('');

  constructor(private userService: UserService) { }


  ngOnInit(): void {
    this.sub = this.userService.getAllUsers().subscribe(u => {
      this.users = u;
      this.users$ = this.filter.valueChanges.pipe(
        startWith(this.filter.value),
        map(text => { return this.users.filter(user => user.username.toLowerCase().includes(text.toLowerCase())) }))
    });
  }

  deleteUser(id: number) {
    this.userService.deleteUser(id).subscribe(() => this.ngOnInit());
  }

  promoteUser(id: number) {
    this.userService.promoteUser(id).subscribe(() => this.ngOnInit());
  }

  unpromoteUser(id: number) {
    this.userService.unpromoteUser(id).subscribe(() => this.ngOnInit());
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

}
