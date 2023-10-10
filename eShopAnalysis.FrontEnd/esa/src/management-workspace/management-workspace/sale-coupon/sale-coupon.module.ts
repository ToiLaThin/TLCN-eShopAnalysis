import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SaleSidenavLinksComponent } from './sale-sidenav-links/sale-sidenav-links.component';
import { SaleCouponComponent } from './sale-coupon.component';
import { SharedModule } from 'src/shared/shared.module';
import { RouterModule, Routes } from '@angular/router';
import { SaleListComponent } from './sale-list/sale-list.component';
import { SaleListInfoDetailComponent } from './sale-list/sale-list-info-detail/sale-list-info-detail.component';
import { SaleAddComponent } from './sale-list/sale-add/sale-add.component';

const saleCouponRoutes: Routes = [
  {
    path: '',
    component: SaleCouponComponent,
    pathMatch: 'prefix',
    children: [
      {
        path: 'sale-list',
        component: SaleListComponent,
        outlet: 'primary',
        pathMatch: 'full',
      }
    ]
  }
]

@NgModule({
  declarations: [
    SaleCouponComponent,
    SaleSidenavLinksComponent,
    SaleListComponent,
    SaleListInfoDetailComponent,
    SaleAddComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(saleCouponRoutes),
    SharedModule
  ]
})
export class SaleCouponModule { }
