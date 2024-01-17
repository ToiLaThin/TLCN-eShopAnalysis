import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagementWorkspaceComponent } from './management-workspace.component';
import { RouterModule, Routes, CanActivate } from '@angular/router';
import { SharedModule } from 'src/shared/shared.module';
import { RoleGuard } from 'src/shared/guards/role.guard';
import { AuthenticatedGuard } from 'src/shared/guards/authenticated.guard';


const managementWorkspaceRoutes: Routes = [
  {
    path:'',
    component: ManagementWorkspaceComponent,
    
    canActivate: [AuthenticatedGuard],
    
    children: [
      {
        path: 'product-catalog',
        loadChildren: () => import('./product-catalog/product-catalog.module').then(m => m.ProductCatalogModule),
        outlet: 'primary',
      },   
      {
        path: 'provider-stock',
        loadChildren: () => import('./provider-stock/provider-stock.module').then(m => m.ProviderStockModule),
        outlet: 'primary',
      },
      {
        path: 'sale-coupon',
        loadChildren: () => import('./sale-coupon/sale-coupon.module').then(m => m.SaleCouponModule),
        outlet: 'primary',
      },
      {
        path: 'order',
        loadChildren: () => import('./order/order.module').then(m => m.OrderModule),
        outlet: 'primary',
      }
    ]
  }
]

@NgModule({
  declarations: [
    ManagementWorkspaceComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(managementWorkspaceRoutes),
    SharedModule,
  ]
})
export class ManagementWorkspaceModule { }
