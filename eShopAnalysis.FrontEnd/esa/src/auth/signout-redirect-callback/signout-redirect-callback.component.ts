import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/shared/services/auth.service';

@Component({
  selector: 'esa-signout-redirect-callback',
  template: ``,
  styles: [``]
})
export class SignoutRedirectCallbackComponent implements OnInit {

  constructor(private _authService: AuthService, private _router: Router) { }

  ngOnInit(): void {
    this._authService.finishLogout().then(() => {
      this._router.navigate(['/'], { replaceUrl: true });
    });
  }

}
