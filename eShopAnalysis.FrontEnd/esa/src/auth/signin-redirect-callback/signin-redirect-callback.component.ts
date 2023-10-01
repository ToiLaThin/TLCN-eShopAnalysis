import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/shared/services/auth.service';

@Component({
  selector: 'esa-signin-redirect-callback',
  template: ``,
  styles: []
})
export class SigninRedirectCallbackComponent implements OnInit {

  constructor(private _authService: AuthService, private _router: Router) { }

  ngOnInit(): void {
    this._authService.finishLogin().then(() => {
      this._router.navigate(['/'], { replaceUrl: true });
    });
  }

}
