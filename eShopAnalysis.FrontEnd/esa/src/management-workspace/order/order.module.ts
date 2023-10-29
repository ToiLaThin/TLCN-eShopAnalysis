import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { OrderComponent } from './order.component';
import { OrderSidenavLinksComponent } from './order-sidenav-links/order-sidenav-links.component';
import { OrderApproveComponent } from './order-approve/order-approve.component';
import { SharedModule } from 'src/shared/shared.module';
import { OrderApproveService } from './order-approve/order-approve.service';

const orderRoutes: Routes = [
  {
    path:'',
    component: OrderComponent,
    pathMatch: 'prefix',
    children: [
      {
        path: 'approve',
        component: OrderApproveComponent,
        outlet: 'primary'
      }
    ]
  }
]

@NgModule({
  declarations: [
    OrderComponent,
    OrderSidenavLinksComponent,
    OrderApproveComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(orderRoutes),
    SharedModule
  ],
  providers: [
    OrderApproveService
  ],
})
export class OrderModule { }
