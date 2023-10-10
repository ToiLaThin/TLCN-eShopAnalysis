import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShoppingComponent } from './shopping/shopping.component';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './shopping/product-list/product-list.component';
import { SharedModule } from 'src/shared/shared.module';
import { ProductListCardComponent } from './shopping/product-list/product-list-card/product-list-card.component';

const shoppingRoutes: Routes = [
  {
    path:'',
    pathMatch:'full',
    component: ShoppingComponent
  }
]

@NgModule({
  declarations: [
    ShoppingComponent,
    ProductListComponent,
    ProductListCardComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(shoppingRoutes),
    SharedModule,
  ]
})
export class ShoppingModule { }
