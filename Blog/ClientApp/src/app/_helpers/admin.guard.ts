import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';
import { CurrentUserService } from '../_services/current-user.service';


@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
    constructor(
        private router: Router,
        private currentUserService: CurrentUserService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        const currentUser = this.currentUserService.getCurrentUser()
        if (currentUser.role == 'Admin') {
            return true;
        }

        this.router.navigate(['/home'], { queryParams: { returnUrl: state.url } });
        return false;
    }
}