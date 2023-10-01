import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UnAuthorizedComponent } from './un-authorized/un-authorized.component';
import { RouterModule, Routes } from '@angular/router';
import { SigninRedirectCallbackComponent } from './signin-redirect-callback/signin-redirect-callback.component';
import { SignoutRedirectCallbackComponent } from './signout-redirect-callback/signout-redirect-callback.component';
import { SharedModule } from 'src/shared/shared.module';

const authRoutes: Routes = [
  {
    path: 'unauthorized',
    component: UnAuthorizedComponent,
    pathMatch: 'full'
  },
  {
    path: 'signin-oidc',
    component: SigninRedirectCallbackComponent,
    pathMatch: 'full'
  },
  {
    path: 'signout-oidc',
    component: SignoutRedirectCallbackComponent,
    pathMatch: 'full'
  }
];


@NgModule({
  declarations: [
    UnAuthorizedComponent,
    SigninRedirectCallbackComponent,
    SignoutRedirectCallbackComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(authRoutes),
    SharedModule
  ],
})
export class AuthModule { }
