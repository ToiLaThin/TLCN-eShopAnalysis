import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/shared/services/auth.service';
import { BookmarkHttpService } from 'src/shared/services/http/bookmark-http.service';
import { LikeProductHttpService } from 'src/shared/services/http/like-product-http.service';
import { RateProductHttpService } from 'src/shared/services/http/rate-product-http.service';
import { RewardPointHttpService } from 'src/shared/services/http/reward-point-http.service';
import { SignalrService } from 'src/shared/services/signalr.service';

@Component({
  selector: 'esa-signin-redirect-callback',
  template: ``,
  styles: []
})
export class SigninRedirectCallbackComponent implements OnInit {

  constructor(
    private _authService: AuthService, 
    private _router: Router, 
    private _signalrService: SignalrService, 
    private _rewardService: RewardPointHttpService,
    private _likeProductService: LikeProductHttpService,
    private _rateProductService: RateProductHttpService,
    private _bookmarkProductService: BookmarkHttpService
    ) { }

  ngOnInit(): void {
    this._authService.finishLogin().then(() => {
      this._signalrService.initConnection();
      this._rewardService.GetCurrentUserRewardPoint(); //will load user reward point to the frontend from backend through http
      this._likeProductService.GetLikeProductMappings(); //will load user liked product to the frontend from backend through http
      this._bookmarkProductService.GetBookmarkProductMappings(); //will load user bookmarked product to the frontend from backend through http
      this._rateProductService.GetUserProductRateMappings(); //will load user rated product to the frontend from backend through http
      this._router.navigate(['/'], { replaceUrl: true });
    });
  }

}
