import { Component, OnInit } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AuthService } from 'src/shared/services/auth.service';
import { CartHttpService } from 'src/shared/services/http/cart-http.service';
import { OrderHttpService } from 'src/shared/services/http/order-http.service';
import { ShowDropDownService } from 'src/shared/services/show-drop-down.service';
import { ThemeToggleService } from 'src/shared/services/theme-toggle.service';
import { AuthStatus } from 'src/shared/types/auth-status.enum';
import { RewardPointHttpService } from 'src/shared/services/http/reward-point-http.service';

@Component({
  selector: 'esa-h-navbar-options',
  templateUrl: './h-navbar-options.component.html',
  styleUrls: ['./h-navbar-options.component.scss']
})
export class HNavbarOptionsComponent implements OnInit {

  showDropDown$!: Observable<boolean>;
  isDarkTheme$!: Observable<boolean>;
  userName$!: Observable<string>;
  userRole$!: Observable<string>;
  authStatus$!: Observable<AuthStatus>;
  cartItemsCount$!: Observable<number>;
  isTrackingOrder$!: Observable<boolean>;
  userRewardPoint$!: Observable<number | undefined>;
  get AuthStatus() { return AuthStatus; } //for template to use enum

  constructor(private showDropDownService: ShowDropDownService, 
              private toggleThemeService: ThemeToggleService, 
              private authService: AuthService,
              private cartService: CartHttpService,
              private orderService: OrderHttpService,
              private rewardService: RewardPointHttpService) { 
      this.showDropDown$ = this.showDropDownService.showHNavBarDropDown$;
      this.isDarkTheme$ = this.toggleThemeService.isDarkTheme$;
      this.userName$ = this.authService.userName$;
      this.userRole$ = this.authService.userRole$;
      this.authStatus$ = this.authService.authStatusGetter$;
      this.cartItemsCount$ = this.cartService.itemsInCartCount$;
      this.isTrackingOrder$ = this.orderService.trackingOrder$.pipe(
        map((order) => {
          if (order === null) return false;
          return true;
        })
      );            
      this.userRewardPoint$ = this.rewardService.userRewardPoint$.pipe(
        map((uRewardPoint) => {
          return uRewardPoint?.rewardPoint; 
        })
      );
      
  }
  ngOnInit(): void {
    
  }
  
  toggleTheme() {
    this.toggleThemeService.toggleTheme();
  }

  toggleDropDown() {
    this.showDropDownService.toggleHNavBarDropDown();    
  }

  login() { this.authService.login(); }
  logout() { this.authService.logout();}

  continueOrderingProcess() {
    let isLoginedAndHavingOrderTracked = this.authService.authStatusGetter$.value === AuthStatus.Authenticated 
                             && this.orderService.trackingOrderGetter !== null;
    if (isLoginedAndHavingOrderTracked) {
      this.orderService.goBackToWhereYouLeftOf();
    } else {
      alert('Please login and have an order to continue');
      return;
    }    
  }
}
