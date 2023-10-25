import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShoppingComponent } from './shopping/shopping.component';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './shopping/product-list/product-list.component';
import { SharedModule } from 'src/shared/shared.module';
import { ProductListCardComponent } from './shopping/product-list/product-list-card/product-list-card.component';
import { ProductDetailComponent } from './shopping/product-detail/product-detail.component';
import { CartListComponent } from './shopping/cart-list/cart-list.component';
import { AuthenticatedGuard } from 'src/shared/guards/authenticated.guard';
import { OrderingInfoConfirmComponent } from './shopping/ordering-process/ordering-info-confirm/ordering-info-confirm.component';
import { OrderDraftListComponent } from './shopping/ordering-process/order-draft-list/order-draft-list.component';
import { PickPaymentMethodComponent } from './shopping/ordering-process/pick-payment-method/pick-payment-method.component';
import { NotifyCustomerObserveOrderComponent } from './shopping/ordering-process/notify-customer-observe-order/notify-customer-observe-order.component';
import { CheckoutRedirectComponent } from './shopping/ordering-process/checkout-redirect/checkout-redirect.component';

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
    canActivate: [AuthenticatedGuard],
    path: 'cart-list',
    component: CartListComponent,
    pathMatch: 'full'
  },
  {    
    canActivate: [AuthenticatedGuard],
    path: 'ordering-info-confirm/:orderId',
    component: OrderingInfoConfirmComponent,
    pathMatch: 'full'
  },
  {
    canActivate: [AuthenticatedGuard],
    path: 'order-draft-list',
    component: OrderDraftListComponent,
    pathMatch: 'full'
  },
  {
    path: 'pick-payment-method',
    component: PickPaymentMethodComponent,
    pathMatch: 'full'
  },
  {
    path: 'checkout-redirect',
    component: CheckoutRedirectComponent,
    pathMatch: 'full'
  },
  {
    path: 'notify-customer-observe-order',
    component: NotifyCustomerObserveOrderComponent,
    pathMatch: 'full'
  }
]

@NgModule({
  declarations: [
    ShoppingComponent,
    ProductListComponent,
    ProductListCardComponent,
    ProductDetailComponent,
    CartListComponent,
    OrderingInfoConfirmComponent,
    OrderDraftListComponent,
    PickPaymentMethodComponent,
    NotifyCustomerObserveOrderComponent,
    CheckoutRedirectComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(shoppingRoutes),
    SharedModule,
  ],
  bootstrap: [ShoppingComponent]
})
export class ShoppingModule { }
