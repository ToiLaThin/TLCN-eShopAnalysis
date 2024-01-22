import { Injectable, OnInit } from '@angular/core';
import { AuthService } from '../auth.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { IBookmarkProduct } from 'src/shared/models/productInteractions.interface';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment as env } from 'src/environments/environment';
import { AuthStatus } from 'src/shared/types/auth-status.enum';

@Injectable({
  providedIn: 'root'
})
//service to bookmark product for curr user
export class BookmarkHttpService implements OnInit{

  constructor(
    private http: HttpClient,
    private _authService: AuthService
  ) { }

  ngOnInit(): void {
    if (this._authService.authStatus === AuthStatus.Authenticated) {
      this.getBookmarkProductMappings().subscribe((bookmarkProductMappings) => {
        console.log("From GetBookmarkProductMappings of bookmark-product-http service: getted", bookmarkProductMappings);
        this.userBookmarkProductMappingsSub.next(bookmarkProductMappings);
      });
    }
  }

  private userBookmarkProductMappingsSub: BehaviorSubject<IBookmarkProduct[]> = new BehaviorSubject<IBookmarkProduct[]>([]);
  userBookmarkProductMappings$: Observable<IBookmarkProduct[]> = this.userBookmarkProductMappingsSub.asObservable();

  public get UserBookmarkedProductBusinessKeys() {
    if (this.userBookmarkProductMappingsSub.value === null) {
      return [];
    }
    return this.userBookmarkProductMappingsSub.value.map((bookmarkProduct) => bookmarkProduct.productBusinessKey);
  }

  public IsProductBookmarked(productBusinessKey: string) {
    return this.UserBookmarkedProductBusinessKeys.includes(productBusinessKey);
  }

  public getBookmarkProductMappings(): Observable<IBookmarkProduct[]> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
    });
    return this.http.get<IBookmarkProduct[]>(`${env.BASEURL}/api/ProductInteraction/BookmarkAPI/GetProductBookmarkedOfUser`, { headers: headers });
  }

  public bookmarkProduct(productBusinessKey: string): Observable<IBookmarkProduct> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
      "productBusinessKey": productBusinessKey,
    });
    return this.http.post<IBookmarkProduct>(`${env.BASEURL}/api/ProductInteraction/BookmarkAPI/BookmarkProductFromUser`, "" ,{ headers: headers });
  }

  public unBookmarkProduct(productBusinessKey: string): Observable<IBookmarkProduct> {
    let headers = new HttpHeaders({
      "userId": this._authService.userId,
      "productBusinessKey": productBusinessKey,
    });
    return this.http.delete<IBookmarkProduct>(`${env.BASEURL}/api/ProductInteraction/BookmarkAPI/UnBookmarkProductFromUser`, { headers: headers });
  }


  //https://angular.io/guide/http-request-data-from-server
  //observable from http does not need to be unsubscribed
  public GetBookmarkProductMappings() {
    this.getBookmarkProductMappings().subscribe((bookmarkProductMappings) => {
      console.log("From GetBookmarkProductMappings of bookmark-product-http service: getted", bookmarkProductMappings);
      this.userBookmarkProductMappingsSub.next(bookmarkProductMappings);
    });
  }

  public ClearUserBookmarkProductMappings() {
    this.userBookmarkProductMappingsSub.next([]);
  }
}
