import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatCardModule } from '@angular/material/card';
import { MatMenuModule } from '@angular/material/menu';
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
    LayoutModule,
    RouterModule,
    SharedModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatIconModule,
    MatListModule,
    MatGridListModule,
    MatCardModule,
    MatMenuModule
  ],
  exports:[
    HNavbarComponent,
    MainComponent
  ]
})
export class CoreModule { }
