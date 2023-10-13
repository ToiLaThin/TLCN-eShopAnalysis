import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { ICartConfirmRequest, ICartItem } from 'src/shared/models/cartItem.interface';
import { environment as env } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CartHttpService {

  itemsInCartSubject!: BehaviorSubject<ICartItem[]>;
  itemsInCart$!: Observable<ICartItem[]>;
  itemsInCartCount$!: Observable<number>;
  itemsInCartKey = 'itemsInCart';
  
  constructor(private http: HttpClient) { 
    this.itemsInCartSubject = new BehaviorSubject<ICartItem[]>(JSON.parse(localStorage.getItem(this.itemsInCartKey) || '[]'));
    this.itemsInCart$ = this.itemsInCartSubject.asObservable();
    this.itemsInCartCount$ = this.itemsInCartSubject.pipe(map(items => items.length));
  }

  //if item exists in cart, update the quantity and finalPrice, even the finalAfterSalePrice
  public upsertItemToCart(cartItem: ICartItem) {
    let itemsInCart = this.itemsInCartSubject.getValue();
    let item = itemsInCart.find(item => item.productModelId === cartItem.productModelId && item.productId === cartItem.productId);
    if (item) { 
      //if product price or sale is update, we have to update both products and cart, will use signalR later
      item.quantity = item.quantity + cartItem.quantity;
      item.finalPrice = item.finalPrice + cartItem.finalPrice;
      item.finalAfterSalePrice = item.finalAfterSalePrice && cartItem.finalAfterSalePrice ? item.finalAfterSalePrice + cartItem.finalAfterSalePrice : undefined;
    } else {
      itemsInCart.push(cartItem);
    }
    this.itemsInCartSubject.next(itemsInCart);
    localStorage.setItem(this.itemsInCartKey, JSON.stringify(itemsInCart));
  }

  public confirmCart(cartConfirmRequest: ICartConfirmRequest) : Observable<any>{
    return this.http.post<ICartConfirmRequest>(`${env.BASEURL}/api/OrderCart/CartAPI/AddCart`, cartConfirmRequest);
  }

}
