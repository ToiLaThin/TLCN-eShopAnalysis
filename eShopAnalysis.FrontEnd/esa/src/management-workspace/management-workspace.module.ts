import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagementWorkspaceComponent } from './management-workspace/management-workspace.component';
import { RouterModule, Routes, CanActivate } from '@angular/router';
import { SharedModule } from 'src/shared/shared.module';
import { RoleGuard } from 'src/shared/guards/role.guard';
import { AuthenticatedGuard } from 'src/shared/guards/authenticated.guard';
import { ProductCatalogModule } from './management-workspace/product-catalog/product-catalog.module';


const managementWorkspaceRoutes: Routes = [
  {
    path:'',
    component: ManagementWorkspaceComponent,

    canActivate: [AuthenticatedGuard],
    
    children: [
      {
        path: 'product-catalog',
        loadChildren: () => import('./management-workspace/product-catalog/product-catalog.module').then(m => m.ProductCatalogModule),
        outlet: 'primary',
      },      
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
