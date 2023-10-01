import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  isNotUser!: boolean;
  constructor(private authService: AuthService, private router: Router) { 
    var isNotUser : boolean = this.authService.userRole$.value === 'admin' ? true : false;
  }
  
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (this.isNotUser) { //is user
      console.log('is trying to get this woman');
      this.router.navigateByUrl('/shopping');
    }
    return true;
  }
  
}
