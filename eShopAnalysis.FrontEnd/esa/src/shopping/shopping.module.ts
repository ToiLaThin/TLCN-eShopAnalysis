import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShoppingComponent } from './shopping/shopping.component';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './shopping/product-list/product-list.component';
import { SharedModule } from 'src/shared/shared.module';
import { ProductListCardComponent } from './shopping/product-list/product-list-card/product-list-card.component';
import { ProductDetailComponent } from './shopping/product-detail/product-detail.component';
import { CartListComponent } from './shopping/cart-list/cart-list.component';

const shoppingRoutes: Routes = [
  {
    path:'',
    pathMatch: 'prefix',
    redirectTo: 'product-list',
  },
  {
    path: 'product-list',
    component: ProductListComponent,
    pathMatch: 'full'
  },
  {
    path: 'product-detail/:productId',
    component: ProductDetailComponent,
    pathMatch: 'full'
  },
  {
    path: 'cart-list',
    component: CartListComponent,
    pathMatch: 'full'
  }
]

@NgModule({
  declarations: [
    ShoppingComponent,
    ProductListComponent,
    ProductListCardComponent,
    ProductDetailComponent,
    CartListComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(shoppingRoutes),
    SharedModule,
  ],
  bootstrap: [ShoppingComponent]
})
export class ShoppingModule { }
