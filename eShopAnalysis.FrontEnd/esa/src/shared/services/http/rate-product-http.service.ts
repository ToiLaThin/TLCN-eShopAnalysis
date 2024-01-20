import { Injectable, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { IRateProduct } from 'src/shared/models/productInteractions.interface';
import { AuthStatus } from 'src/shared/types/auth-status.enum';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment as env } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RateProductHttpService implements OnInit{

  constructor(
    private http: HttpClient, 
    private _authService: AuthService
  ) { }

  private userProductRateMappings: BehaviorSubject<IRateProduct[]> = new BehaviorSubject<IRateProduct[]>([]);
  userProductRateMappings$ = this.userProductRateMappings.asObservable();

  ngOnInit(): void {
    if (this._authService.authStatus === AuthStatus.Authenticated) {
      this.getUserProductRateMappings().subscribe((userProductRateMappings) => {
        this.userProductRateMappings.next(userProductRateMappings);
      });
    }
  }

  public getUserProductRateMappings(): Observable<IRateProduct[]> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
    });
    return this.http.get<IRateProduct[]>(`${env.BASEURL}/api/ProductInteraction/RateAPI/GetRatedMappingsOfUser`, { headers: headers });
  }

  public rateProduct(productBusinessKey: string, rating: string): Observable<IRateProduct> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
      "productBusinessKey": productBusinessKey,
      "rating": rating.toString(), //will be parse to double in backend
    });
    return this.http.post<IRateProduct>(`${env.BASEURL}/api/ProductInteraction/RateAPI/RateProductFromUser`, "",{ headers: headers });
  }

  public IsProductRated(productBusinessKey: string) : boolean {
    if (this.userProductRateMappings.value === null) {
      return false;
    }
    return this.userProductRateMappings.value
    .map((rateProduct) => rateProduct.productBusinessKey)
    .includes(productBusinessKey);
  }

  public getProductRating(productBusinessKey: string) : number | undefined {
    if (this.userProductRateMappings.value === null) {
      return 0;
    }
    let rateProduct = this.userProductRateMappings.value
    .find((rateProduct) => rateProduct.productBusinessKey === productBusinessKey);
    return rateProduct?.rating;
  }

  //https://angular.io/guide/http-request-data-from-server
  //observable from http does not need to be unsubscribed
  public GetUserProductRateMappings() {
    this.getUserProductRateMappings().subscribe((userProductRateMappings) => {
      this.userProductRateMappings.next(userProductRateMappings);
    });
  }

  public ClearUserProductRateMappings() {
    this.userProductRateMappings.next([]);
  }
}
