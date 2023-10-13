import { Component, OnInit } from '@angular/core';
import { CartHttpService } from 'src/shared/services/http/cart-http.service';
import { Observable } from 'rxjs';
import { ICartConfirmRequest, ICartItem } from 'src/shared/models/cartItem.interface';
import { AuthService } from './../../../shared/services/auth.service';

@Component({
  selector: 'esa-cart-list',
  templateUrl: './cart-list.component.html',
  styleUrls: ['./cart-list.component.scss']
})
export class CartListComponent implements OnInit {

  cartItems$!: Observable<ICartItem[]>;
  constructor(private cartService: CartHttpService, private authService: AuthService) { }

  ngOnInit(): void {
    this.cartItems$ = this.cartService.itemsInCart$;
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

}
