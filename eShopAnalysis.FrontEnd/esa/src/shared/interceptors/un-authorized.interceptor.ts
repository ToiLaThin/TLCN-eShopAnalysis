import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
  HttpStatusCode
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { AuthStatus } from '../types/auth-status.enum';


@Injectable()
export class UnAuthorizedInterceptor implements HttpInterceptor {

  constructor(private router: Router, 
              private authService: AuthService) {}



  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((err: HttpErrorResponse) => {
        if(err.status === HttpStatusCode.Unauthorized){ //may not have bearer jwt token or jwt expired
          this.authService.authStatus = AuthStatus.Anonymouse;
          this.authService.clearUserEssentialInfo();
          this.router.navigateByUrl('/auth/unauthorized');
        }
        throw err;
      })
    );
  }
}
