import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path:'',
    pathMatch:'full',
    redirectTo:'shopping'
  },
  {
    path: 'shopping',
    loadChildren: () => import('../shopping/shopping.module').then(m => m.ShoppingModule)
  },
  {
    path: 'management',
    loadChildren: () => import('../management-workspace/management-workspace.module').then(m => m.ManagementWorkspaceModule)
  },
  {
    path: 'auth',
    loadChildren: () => import('../auth/auth.module').then(m => m.AuthModule)
  }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
