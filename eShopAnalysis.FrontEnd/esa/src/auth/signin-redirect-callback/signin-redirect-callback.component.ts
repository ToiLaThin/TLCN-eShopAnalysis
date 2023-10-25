import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/shared/services/auth.service';
import { SignalrService } from 'src/shared/services/signalr.service';

@Component({
  selector: 'esa-signin-redirect-callback',
  template: ``,
  styles: []
})
export class SigninRedirectCallbackComponent implements OnInit {

  constructor(private _authService: AuthService, private _router: Router, private signalrService: SignalrService) { }

  ngOnInit(): void {
    this._authService.finishLogin().then(() => {
      this.signalrService.initConnection();
      this._router.navigate(['/'], { replaceUrl: true });
    });
  }

}
