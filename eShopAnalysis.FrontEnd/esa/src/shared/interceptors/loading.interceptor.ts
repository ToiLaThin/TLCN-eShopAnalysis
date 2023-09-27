import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, finalize } from 'rxjs';
import { ShowLoaderService } from '../services/show-loader.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  private totalRequest = 0;
  constructor(
    private loaderService: ShowLoaderService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.totalRequest++;
    this.loaderService.setIsLoadingSpinner = true;
    return next.handle(request).pipe(
      finalize(() => {
        this.totalRequest--;
        if(this.totalRequest === 0){ this.loaderService.setIsLoadingSpinner = false; }
      })
    );
  }
}
