import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RouterModule } from '@angular/router';
import { HNavbarComponent } from './layouts/h-navbar/h-navbar.component';
import { HNavbarOptionsComponent } from './layouts/h-navbar/h-navbar-options/h-navbar-options.component';
import { SharedModule } from 'src/shared/shared.module';
import { MainComponent } from './layouts/main/main.component';



@NgModule({
  declarations: [
    HNavbarComponent,
    HNavbarOptionsComponent,
    MainComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,    
  ],
  exports:[
    HNavbarComponent,
    MainComponent
  ]
})
export class CoreModule { }
