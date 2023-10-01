import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, tap } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { AuthStatus } from '../types/auth-status.enum';
import { Router } from '@angular/router';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService,private router: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let cloneRequest: HttpRequest<unknown> = request;
    //if logined , add bearer token to header, if not not bearer token added
    if (this.authService.authStatus === AuthStatus.Authenticated) {
      cloneRequest = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.authService.accessToken}`
        }
      });
    }
    return next.handle(cloneRequest);
  }
}
