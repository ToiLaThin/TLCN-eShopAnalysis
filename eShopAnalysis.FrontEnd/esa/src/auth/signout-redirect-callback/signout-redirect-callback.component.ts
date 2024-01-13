import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/shared/services/auth.service';
import { LikeProductHttpService } from 'src/shared/services/http/like-product-http.service';
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
    private _rewardService: RewardPointHttpService,
    private _likeProductService: LikeProductHttpService
    ) { }

  ngOnInit(): void {
    this._authService.finishLogout().then(() => {
      this._signalrService.stopConnection();
      this._rewardService.ClearUserRewardPoint();
      this._likeProductService.ClearLikeProductMappings();
      this._router.navigate(['/'], { replaceUrl: true });
    });
  }

}
