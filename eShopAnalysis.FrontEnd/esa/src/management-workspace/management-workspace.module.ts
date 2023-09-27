import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagementWorkspaceComponent } from './management-workspace/management-workspace.component';
import { RouterModule, Routes, CanActivate } from '@angular/router';
import { SharedModule } from 'src/shared/shared.module';
import { ProductAddComponent } from './management-workspace/product-add/product-add.component';
import { RoleGuard } from 'src/shared/guards/role.guard';
import { AuthenticatedGuard } from 'src/shared/guards/authenticated.guard';


const managementWorkspaceRoutes: Routes = [
  {
    path:'',
    component: ManagementWorkspaceComponent,

    canActivate: [AuthenticatedGuard],
    canActivateChild: [RoleGuard],
    children: [
      {
        path: 'product-add',
        component: ProductAddComponent,
        outlet: 'primary',
        pathMatch: 'full',

      }
    ]
  }
]

@NgModule({
  declarations: [
    ManagementWorkspaceComponent,
    ProductAddComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(managementWorkspaceRoutes),
    SharedModule,
  ]
})
export class ManagementWorkspaceModule { }
