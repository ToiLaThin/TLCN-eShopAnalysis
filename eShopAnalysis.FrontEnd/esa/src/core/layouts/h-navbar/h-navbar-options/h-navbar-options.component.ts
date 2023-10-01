import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from 'src/shared/services/auth.service';
import { ShowDropDownService } from 'src/shared/services/show-drop-down.service';
import { ThemeToggleService } from 'src/shared/services/theme-toggle.service';
import { AuthStatus } from 'src/shared/types/auth-status.enum';

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
  get AuthStatus() { return AuthStatus; } //for template to use enum

  constructor(private showDropDownService: ShowDropDownService, 
              private toggleThemeService: ThemeToggleService, 
              private authService: AuthService) { 
      this.showDropDown$ = this.showDropDownService.showHNavBarDropDown$;
      this.isDarkTheme$ = this.toggleThemeService.isDarkTheme$;
      this.userName$ = this.authService.userName$;
      this.userRole$ = this.authService.userRole$;
      this.authStatus$ = this.authService.authStatusGetter$;
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
}
