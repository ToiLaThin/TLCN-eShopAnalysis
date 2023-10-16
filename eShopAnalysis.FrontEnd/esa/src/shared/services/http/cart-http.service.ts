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
  discountAmountByCoupon$!: BehaviorSubject<number>;  
  couponApplied$!: BehaviorSubject<boolean>;
  couponCodeApplied!: BehaviorSubject<string | undefined>;
  itemsInCartKey = 'itemsInCart';
  
  constructor(private http: HttpClient) { 
    this.itemsInCartSubject = new BehaviorSubject<ICartItem[]>(JSON.parse(localStorage.getItem(this.itemsInCartKey) || '[]'));
    this.itemsInCart$ = this.itemsInCartSubject.asObservable();
    this.itemsInCartCount$ = this.itemsInCartSubject.pipe(map(items => items.length));
    this.discountAmountByCoupon$ = new BehaviorSubject<number>(0);
    this.couponApplied$ = new BehaviorSubject<boolean>(false);
    this.couponCodeApplied = new BehaviorSubject<string | undefined>(undefined);
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

  public changeCartItemQuantity(indexInCart: number, newQuantity: number) {
    let currentItemsInCart = this.itemsInCartSubject.getValue();
    let toUpdateQuantityItem = currentItemsInCart[indexInCart];

    toUpdateQuantityItem.finalPrice = toUpdateQuantityItem.finalPrice / toUpdateQuantityItem.quantity * newQuantity; //divide by old quantity and multiply by new quantity
    toUpdateQuantityItem.finalAfterSalePrice = toUpdateQuantityItem.finalAfterSalePrice ? toUpdateQuantityItem.finalAfterSalePrice / toUpdateQuantityItem.quantity * newQuantity 
                                                                                        : undefined;
    toUpdateQuantityItem.quantity = newQuantity;

    this.itemsInCartSubject.next(currentItemsInCart);//subject next the updated cart
    localStorage.setItem(this.itemsInCartKey, JSON.stringify(currentItemsInCart));
  }

  removeCartItemFromCart(indexInCart: number) {
    let currentItemsInCart = this.itemsInCartSubject.getValue();
    currentItemsInCart.splice(indexInCart, 1); //do not assign a var to this, it will be the removed item not the updated cart
    this.itemsInCartSubject.next(currentItemsInCart);//subject next the updated cart    
    localStorage.setItem(this.itemsInCartKey, JSON.stringify(currentItemsInCart));
  }

  applyCouponWithDiscountAmountAndCode(discountAmount: number, couponCode: string) {
    this.discountAmountByCoupon$.next(discountAmount);
    this.couponApplied$.next(true);
    this.couponCodeApplied.next(couponCode);
  }

  removeCouponApplied() {
    this.discountAmountByCoupon$.next(0);
    this.couponApplied$.next(false);
    this.couponCodeApplied.next(undefined);
  }

}
