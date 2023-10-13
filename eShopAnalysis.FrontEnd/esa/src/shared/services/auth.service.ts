import { HttpClient, HttpHeaderResponse, HttpHeaders, HttpParamsOptions } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthStatus } from '../types/auth-status.enum';
import { JwtService } from './jwt.service';
import { Router } from '@angular/router';
import { environment as env } from 'src/environments/environment';
import { UserManager, User, UserManagerSettings, WebStorageStateStore } from 'oidc-client';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private _userManager: UserManager;
  private _user!: User;
  private get idpSettings() : UserManagerSettings {
    return {
      authority: `${env.BASEURL}/Auth/IdentityServer/`,      
      //phải là localhost:5143 chứ ko đi qua ocelot(localhost:7003 vì nó sẽ đưa vào auth.identityserver 
      //là container mà brower ko biết đường dẫn đến container đó => 
      //gây lỗi dns probe finished nxdomain(có config dns, extra host hoặc cả host.docker.internal cũng ko được))
      metadataUrl: `${env.BASEURL}/Auth/IdentityServer/.well-known/openid-configuration`,
      client_id: env.CLIENTID,
      redirect_uri: `${env.CLIENTROOT}/signin-oidc`,
      post_logout_redirect_uri: `${env.CLIENTROOT}/signout-oidc`,
      scope: "openid profile MyApi.Scope User.Info",
      response_type: "code",
      userStore: new WebStorageStateStore({ store: localStorage })
      //lưu user vào localStorage vì session tắt tab sẽ mất
    }
  }

  private authStatus$: BehaviorSubject<AuthStatus> = new BehaviorSubject<AuthStatus>(AuthStatus.Anonymouse);
  userName$: BehaviorSubject<string> = new BehaviorSubject<string>('');
  userRole$: BehaviorSubject<string> = new BehaviorSubject<string>('');
  
  get idToken() { this.checkLoginStatus(); return this._user.id_token; }
  get accessToken() { this.checkLoginStatus(); return this._user.access_token; }
  get authStatus() : AuthStatus { //mỗi lần get phải check lại user có thay đổi không
    return this.checkLoginStatus();
  }  
  get authStatusGetter$() : BehaviorSubject<AuthStatus> { return this.authStatus$; } //for other component observable to subscribe will be used to check if user authenticated or not
  set authStatus(status: AuthStatus) { this.authStatus$.next(status); }
  get userId() { return this._user.profile.sub; }
  
  constructor(private http: HttpClient, 
    private jwtService: JwtService,
    private router: Router) { 

    this._userManager = new UserManager(this.idpSettings);
    // this.checkLoginStatus();
  }  

  public checkLoginStatus() : AuthStatus {
    //promise
    this._userManager.getUser().then((user) => { 
      if (this._user !== user) {
        if (user) {
          if (user.expired) { 
            alert('Your session has expired. Please login again.');
            this.logout();
          }
          this._user = user;
          this.authStatus = AuthStatus.Authenticated;
          this.setUserEssentialInfo(this._user.id_token!);
        }
        else { //this.logout() ?
          this._user = null!;
          this.authStatus = AuthStatus.Anonymouse;
          this.clearUserEssentialInfo();
        }
      }
    });
    return this.authStatus$.value;
  }

  login() { return this._userManager.signinRedirect(); }

  finishLogin() : Promise<User> {
    return this._userManager.signinRedirectCallback().then((user) => {
      this._user = user!;
      console.log("this is the user: ",this._user);
      //have Identity.Cookie =>delete it so that when user logout, login again we will have to login again
      //if server set lifetime for cookie, we do not need to delete it
      // console.log(window.document.cookie);
      // window.document.cookie = "Identity.Cookie=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";

      
      this.authStatus = AuthStatus.Authenticated;
      this.setUserEssentialInfo(this._user.id_token!);
      return user!;
    });
  }

  logout() {
    return this._userManager.signoutRedirect();
    //In backend default controller for logout is AccountController
    //No webpage was found for the web address: https://localhost:7134/Account/Logout?logoutId=, if i delete cookieConfig.LogoutPath = "/Auth/Logout" in IdentityServer, it will redirect to this url
  }

  finishLogout() {
    return this._userManager.signoutRedirectCallback().then(() => {
      this._user = null!;
      this.authStatus = AuthStatus.Anonymouse;
      this.clearUserEssentialInfo();
    });
  }

  setUserEssentialInfo(idToken: string) {
    const decodedIdToken: object = this.jwtService.decode(idToken);
      this.userName$.next((decodedIdToken as any).name);
      this.userRole$.next((decodedIdToken as any).role);
  }

  clearUserEssentialInfo() {
    this.userName$.next('');
    this.userRole$.next('');
  }
}
