import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SharedModule } from 'src/shared/shared.module';
import { AuthenticatedGuard } from 'src/shared/guards/authenticated.guard';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';
import { CartListComponent } from './cart-list/cart-list.component';
import { OrderingInfoConfirmComponent } from './ordering-process/ordering-info-confirm/ordering-info-confirm.component';
import { OrderDraftListComponent } from './ordering-process/order-draft-list/order-draft-list.component';
import { PickPaymentMethodComponent } from './ordering-process/pick-payment-method/pick-payment-method.component';
import { CheckoutRedirectComponent } from './ordering-process/checkout-redirect/checkout-redirect.component';
import { NotifyCustomerObserveOrderComponent } from './ordering-process/notify-customer-observe-order/notify-customer-observe-order.component';
import { ShoppingComponent } from './shopping.component';
import { ProductListCardComponent } from './product-list/product-list-card/product-list-card.component';

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
