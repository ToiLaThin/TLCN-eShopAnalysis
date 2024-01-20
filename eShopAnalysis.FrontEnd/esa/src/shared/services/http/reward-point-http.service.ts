import { Injectable } from '@angular/core';
import { AuthService } from '../auth.service';
import { IUserRewardPoint } from 'src/shared/models/rewardPoint.interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment as env } from 'src/environments/environment';
import { AuthStatus } from 'src/shared/types/auth-status.enum';

@Injectable({
  providedIn: 'root'
})
export class RewardPointHttpService {
  constructor(private http: HttpClient, private authService: AuthService) { 
    if (this.authService.authStatus === AuthStatus.Anonymouse) { return; }
    this.getCurrentUserRewardPoint().subscribe((userRewardPoint) => {
      this.userRewardPointSub.next(userRewardPoint);
      console.log("User Reward point: ", userRewardPoint.rewardPoint);
    });
  }

  private userRewardPointSub: BehaviorSubject<IUserRewardPoint | null> = new BehaviorSubject<IUserRewardPoint | null>(null);
  public userRewardPoint$: Observable<IUserRewardPoint | null> = this.userRewardPointSub.asObservable();
  public get CurrUserRewardPoint() { return this.userRewardPointSub.value?.rewardPoint; }
  private getCurrentUserRewardPoint() {
    //TODO use header not query or we will have 404 Not Found, query also expose user id
    let headers = new HttpHeaders( `userId: ${this.authService.userId}` );
    return this.http.get<IUserRewardPoint>(`${env.BASEURL}/api/CustomerLoyaltyProgram/UserRewardPointAPI/GetRewardPointOfUser`, { headers: headers });
  }

  //https://angular.io/guide/http-request-data-from-server
  //observable from http does not need to be unsubscribed
  public GetCurrentUserRewardPoint() {
    //a public version so we can check then call this method
    this.getCurrentUserRewardPoint().subscribe((userRewardPoint) => {
      this.userRewardPointSub.next(userRewardPoint);
      console.log("User Reward point: ", userRewardPoint.rewardPoint);
    });
  }

  public ClearUserRewardPoint() {
    this.userRewardPointSub.next(null);
  }
}
