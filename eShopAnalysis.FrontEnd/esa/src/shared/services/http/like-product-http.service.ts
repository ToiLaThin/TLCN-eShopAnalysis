import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { ILikeProduct, LikeStatus } from 'src/shared/models/productInteractions.interface';
import { AuthService } from '../auth.service';
import { BehaviorSubject, Observable, Subscription, filter, map } from 'rxjs';
import { environment as env } from 'src/environments/environment';
import { AuthStatus } from 'src/shared/types/auth-status.enum';

@Injectable({
  providedIn: 'root'
})
export class LikeProductHttpService implements OnInit{

  constructor(
    private http: HttpClient, 
    private _authService: AuthService) { }
    
  private userLikeProductMappingsSub: BehaviorSubject<ILikeProduct[]> = new BehaviorSubject<ILikeProduct[]>([]);
  userLikeProductMappings$: Observable<ILikeProduct[]> = this.userLikeProductMappingsSub.asObservable();

  ngOnInit(): void {    
    if (this._authService.authStatus === AuthStatus.Authenticated) { 
      this.getLikeProductMappings().subscribe((likeProductMappings) => {
        console.log("From GetLikeProductMappings of like-product-http service: getted", likeProductMappings);
        this.userLikeProductMappingsSub.next(likeProductMappings);
      });
    }
  }

  public get UserLikedProductBusinessKeys() { 
    if (this.userLikeProductMappingsSub.value === null) {
      return [];
    }
    return this.userLikeProductMappingsSub.value
    .filter((likedProducts) => likedProducts.status === LikeStatus.Liked.valueOf())
    .map((likeProduct) => likeProduct.productBusinessKey); 
  }

  public get UserDislikedProductBusinessKeys() {
    if (this.userLikeProductMappingsSub.value === null) {
      return [];
    }
    return this.userLikeProductMappingsSub.value
    .filter((likedProducts) => likedProducts.status === LikeStatus.Disliked.valueOf())
    .map((likeProduct) => likeProduct.productBusinessKey); 
  }

  public IsProductLiked(productBusinessKey: string) {
    return this.UserLikedProductBusinessKeys.includes(productBusinessKey);
  }

  public IsProductDisliked(productBusinessKey: string) {
    return this.UserDislikedProductBusinessKeys.includes(productBusinessKey);
  }

  public likeProduct(productBusinessKey: string): Observable<ILikeProduct> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
      "productBusinessKey": productBusinessKey,
    });
    return this.http.post<ILikeProduct>(`${env.BASEURL}/api/ProductInteraction/LikeAPI/LikeProductFromUser`, "" ,{ headers: headers });
  }

  public unlikeProduct(productBusinessKey: string): Observable<ILikeProduct> {
    let headers = new HttpHeaders()
    headers = headers.set("userId", this._authService.userId)
    headers = headers.set("productBusinessKey", productBusinessKey);
    return this.http.post<ILikeProduct>(`${env.BASEURL}/api/ProductInteraction/LikeAPI/UnLikeProductFromUser`, "" ,{ headers: headers });
  }

  public dislikeProduct(productBusinessKey: string): Observable<ILikeProduct> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
      "productBusinessKey": productBusinessKey,
    });
    return this.http.post<ILikeProduct>(`${env.BASEURL}/api/ProductInteraction/LikeAPI/DisLikeProductFromUser`, "" ,{ headers: headers });
  }
  
  public getLikeProductMappings(): Observable<ILikeProduct[]> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
    });
    return this.http.get<ILikeProduct[]>(`${env.BASEURL}/api/ProductInteraction/LikeAPI/GetUserProductLikedMappingsOfUser`, { headers: headers });
  }

  public GetLikeProductMappings(): Subscription {
    let subscription = this.getLikeProductMappings().subscribe((likedProducts) => {
      this.userLikeProductMappingsSub.next(likedProducts);
    })    
    return subscription;
    //.unsubscribe(); this line will cause the handler inside the subscribe not executed cause error so we return subscription
    // but do not unsubscribe in the component too, because if unsubscribe in the component, the handler inside the subscribe will not be executed when the subscription is still neccessary
    // especially subscription inside service like this
  }

  //used when user logged out => no longer able to indentify user by userId
  public ClearLikeProductMappings() {
    this.userLikeProductMappingsSub.next([]);
  }
}
