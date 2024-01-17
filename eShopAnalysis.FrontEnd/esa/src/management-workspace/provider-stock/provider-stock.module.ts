import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProviderStockComponent } from './provider-stock.component';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from 'src/shared/shared.module';
import { ProviderStockSidenavLinksComponent } from './provider-stock-sidenav-links/provider-stock-sidenav-links.component';
import { ProviderListComponent } from './provider-list/provider-list.component';
import { ProviderDetailComponent } from './provider-detail/provider-detail.component';

const providerStockRoutes: Routes = [
  {
    path: '',
    component: ProviderStockComponent,
    pathMatch: 'prefix',
    children: [
      {
        path: 'provider-list',
        component: ProviderListComponent,
        outlet: 'primary'
      },
      {
        path: 'provider-detail',
        component: ProviderDetailComponent,
        outlet: 'primary'
      }
    ]
  }
]

@NgModule({
  declarations: [
    ProviderStockComponent,
    ProviderStockSidenavLinksComponent,
    ProviderListComponent,
    ProviderDetailComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(providerStockRoutes),
    SharedModule
  ]
})
export class ProviderStockModule { }
