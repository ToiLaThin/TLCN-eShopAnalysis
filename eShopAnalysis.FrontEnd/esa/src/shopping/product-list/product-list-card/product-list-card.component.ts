import { Component, Input, OnInit } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import { IProduct } from 'src/shared/models/product.interface';
import { AuthService } from 'src/shared/services/auth.service';
import { BookmarkHttpService } from 'src/shared/services/http/bookmark-http.service';
import { AuthStatus } from 'src/shared/types/auth-status.enum';

@Component({
  selector: 'esa-product-list-card',
  templateUrl: './product-list-card.component.html',
  styleUrls: ['./product-list-card.component.scss']
})
export class ProductListCardComponent implements OnInit {

  @Input('product') product!: IProduct;

  authStatus$!: Observable<AuthStatus>;
  isProductBookmarked$!: Observable<boolean>;
  public get AuthStatus() { return AuthStatus; }
  
  constructor(
    private authService: AuthService,
    private _bookmarkProductService: BookmarkHttpService,
    ) {
      this.authStatus$ = this.authService.authStatusGetter$;

      this.isProductBookmarked$ = this._bookmarkProductService.userBookmarkProductMappings$.pipe(
        map((bookmarkProductMappings) => {
          if (bookmarkProductMappings === null) {
            return false;
          }
          return bookmarkProductMappings.some((bookmarkProduct) => bookmarkProduct.productBusinessKey === this.product.businessKey);
        }),
        tap((isProductBookmarked) => console.log("Init isProductBookmarked: ", isProductBookmarked))
      );

     }
  
  ngOnInit(): void {
  }


}

