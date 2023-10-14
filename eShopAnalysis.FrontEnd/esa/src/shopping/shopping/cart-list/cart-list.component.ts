import { Component, OnInit } from '@angular/core';
import { CartHttpService } from 'src/shared/services/http/cart-http.service';
import { Observable, combineLatest, map } from 'rxjs';
import { ICartConfirmRequest, ICartItem } from 'src/shared/models/cartItem.interface';
import { AuthService } from './../../../shared/services/auth.service';

@Component({
  selector: 'esa-cart-list',
  templateUrl: './cart-list.component.html',
  styleUrls: ['./cart-list.component.scss']
})
export class CartListComponent implements OnInit {

  cartItems$!: Observable<ICartItem[]>;
  subItemsPrice$!: Observable<number>;
  subItemsAfterSalePrice$!: Observable<number>;
  discountAmount$!: Observable<number>;
  cartItemCount$!: Observable<number>;
  constructor(private cartService: CartHttpService, private authService: AuthService) { }

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
    this.discountAmount$ = combineLatest([this.subItemsPrice$, this.subItemsAfterSalePrice$]).pipe(
      map(([subItemsPrice, subItemsAfterSalePrice]) => subItemsPrice - subItemsAfterSalePrice)
    );
    this.cartItemCount$ = this.cartService.itemsInCartCount$;
  }

  confirmCart() {
    let cartRequest : ICartConfirmRequest = {
      cartItems: this.cartService.itemsInCartSubject.getValue(),
      userId: this.authService.userId
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

}
