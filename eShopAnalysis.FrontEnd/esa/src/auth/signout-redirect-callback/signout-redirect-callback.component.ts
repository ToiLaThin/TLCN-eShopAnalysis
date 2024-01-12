import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/shared/services/auth.service';
import { RewardPointHttpService } from 'src/shared/services/http/reward-point-http.service';
import { SignalrService } from 'src/shared/services/signalr.service';

@Component({
  selector: 'esa-signout-redirect-callback',
  template: ``,
  styles: [``]
})
export class SignoutRedirectCallbackComponent implements OnInit {

  constructor(private _authService: AuthService, 
    private _router: Router, 
    private _signalrService: SignalrService,
    private _rewardService: RewardPointHttpService) { }

  ngOnInit(): void {
    this._authService.finishLogout().then(() => {
      this._signalrService.stopConnection();
      this._rewardService.ClearUserRewardPoint();
      this._router.navigate(['/'], { replaceUrl: true });
    });
  }

}
