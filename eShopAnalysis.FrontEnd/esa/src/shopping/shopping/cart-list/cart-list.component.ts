import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { CartHttpService } from 'src/shared/services/http/cart-http.service';
import { Observable, combineLatest, map } from 'rxjs';
import { ICartConfirmRequest, ICartItem } from 'src/shared/models/cartItem.interface';
import { AuthService } from './../../../shared/services/auth.service';
import { CouponHttpService } from 'src/shared/services/http/coupon-http.service';
import { ICoupon } from 'src/shared/models/coupon.interface';
import { DiscountType } from 'src/shared/models/saleItem.interface';

@Component({
  selector: 'esa-cart-list',
  templateUrl: './cart-list.component.html',
  styleUrls: ['./cart-list.component.scss']
})
export class CartListComponent implements OnInit {

  cartItems$!: Observable<ICartItem[]>;
  subItemsPrice$!: Observable<number>;
  subItemsAfterSalePrice$!: Observable<number>;
  discountAmountSale$!: Observable<number>;
  discountAmountCoupon$!: Observable<number>;
  couponApplied$!: Observable<boolean>;
  subItemsAfterSaleThenCouponPrice$!: Observable<number>;
  cartItemCount$!: Observable<number>;
  allActiveCouponsNotUsedByUser$!: Observable<ICoupon[]>;

  @ViewChild('couponCodeApply', {read: ElementRef}) couponCodeInputted!: ElementRef<HTMLInputElement>;
  constructor(private cartService: CartHttpService, 
              private authService: AuthService, 
              private couponService: CouponHttpService) { }

  ngOnInit(): void {
    this.cartItems$ = this.cartService.itemsInCart$;
    this.subItemsPrice$ = this.cartService.itemsInCart$.pipe(
      map(items => items.reduce((acc, item) => acc + item.finalPrice, 0))
    );
    this.subItemsAfterSalePrice$ = this.cartService.itemsInCart$.pipe(
      map(items => items.reduce((acc, item) => {
          if (item.isOnSale) {
            return acc + item.finalAfterSalePrice!;
          } else { 
            return acc + item.finalPrice; 
          }
        }, 0)
      )
    );   
    this.discountAmountSale$ = combineLatest([this.subItemsPrice$, this.subItemsAfterSalePrice$]).pipe(
      map(([subItemsPrice, subItemsAfterSalePrice]) => subItemsPrice - subItemsAfterSalePrice)
    );
    this.cartItemCount$ = this.cartService.itemsInCartCount$;
    this.allActiveCouponsNotUsedByUser$ = this.couponService.allActiveCouponsNotUsedByUser$;
    this.discountAmountCoupon$ = this.cartService.discountAmountByCoupon$;
    this.subItemsAfterSaleThenCouponPrice$ = combineLatest([this.subItemsAfterSalePrice$, this.discountAmountCoupon$]).pipe(
      map(([subItemsAfterSalePrice, discountAmountCoupon]) => subItemsAfterSalePrice - discountAmountCoupon)
    );
    this.couponApplied$ = this.cartService.couponApplied$;
  }

  confirmCart() {
    let cartRequest : ICartConfirmRequest = {
      cartItems: this.cartService.itemsInCartSubject.getValue(),
      userId: this.authService.userId,
      couponCode: this.cartService.couponCodeApplied.getValue() //undefined if no coupon applied and  will not have couponCode field in CartConfirmRequestDTO in backend
    }
    console.log(cartRequest);
    
    this.cartService.confirmCart(cartRequest).subscribe(
      () => {
        this.cartService.itemsInCartSubject.next([]);
        localStorage.removeItem(this.cartService.itemsInCartKey);
        console.log('cart confirmed');
        
      },
      err => console.log(err)
    );
  }

  changeCartItemQuantity(indexInCart: number, event: Event) {
    let quantity = (event.target as HTMLInputElement).valueAsNumber;
    this.cartService.changeCartItemQuantity(indexInCart, quantity);
  }

  removeCartItemFromCart(indexInCart: number) {
    console.log(indexInCart);    
    this.cartService.removeCartItemFromCart(indexInCart);
  }

  applyCoupon() {
    let inputtedCouponCode = this.couponCodeInputted.nativeElement.value;//already casted to HTMLInputElement in ViewChild        
    let subscription = this.allActiveCouponsNotUsedByUser$.subscribe(
      (coupons) => { 
        let coupon = coupons.find(coupon => coupon.couponCode === inputtedCouponCode);
        if (coupon !== undefined) {
          //TODO: check minimum price now to make sure the coupon the coupon can be applied          
          this.notifyServiceCouponApplied(coupon);
        } else {
          console.log("No coupon valid");
        }
      }
    );
    subscription.unsubscribe(); //unsubscribe after first emit
  }

  private notifyServiceCouponApplied(coupon: ICoupon) {
    //depend on the discount type, we will calculate the discount amount with the discount value
    //then call the cartService to apply the discount amount
    //it will modify the discountAmountByCoupon$ subject and affect the subItemsAfterSaleThenCouponPrice$ observable
    if (coupon.discountType === DiscountType.ByPercent) {
      //discount by percent then we have to get the price after sale to calc the discount amount, which require to subscribe to subItemsAfterSalePrice$
      const couponDiscountValue = coupon.discountValue;
      let tempSub = this.subItemsAfterSalePrice$.subscribe(
        (afterSalePrice) => {
          const discountAmount = afterSalePrice * couponDiscountValue / 100;
          this.cartService.applyCouponWithDiscountAmountAndCode(discountAmount, coupon.couponCode);
        }
      );
      tempSub.unsubscribe();
    } else if (coupon.discountType === DiscountType.ByValue) { //NoDiscount is for sure 
      let discountAmount = coupon.discountValue;
      this.cartService.applyCouponWithDiscountAmountAndCode(discountAmount, coupon.couponCode);
    } else {
      console.log("No discount type valid");
    }
  }

  removeCoupon() {
    this.cartService.removeCouponApplied();
  }

}
