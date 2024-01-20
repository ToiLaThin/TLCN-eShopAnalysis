import { Component, OnDestroy, OnInit } from '@angular/core';
import { CartHttpService } from 'src/shared/services/http/cart-http.service';
import { IProduct, IProductModel } from 'src/shared/models/product.interface';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';
import { Observable, Subscription, find, map, tap } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { LikeProductHttpService } from 'src/shared/services/http/like-product-http.service';
import { LikeStatus } from 'src/shared/models/productInteractions.interface';
import { AuthStatus } from 'src/shared/types/auth-status.enum';
import { AuthService } from 'src/shared/services/auth.service';
import { BookmarkHttpService } from 'src/shared/services/http/bookmark-http.service';
import { RateProductHttpService } from 'src/shared/services/http/rate-product-http.service';
import { CommentProductHttpService } from 'src/shared/services/http/comment-product-http.service';
import { IComment } from 'src/shared/models/order.interface';

@Component({
  selector: 'esa-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit, OnDestroy {

  productSubscription!: Subscription;
  routeParamsSubscription!: Subscription;
  productId!: string;
  product!: IProduct | undefined;
  isProductLiked$!: Observable<boolean>;
  isProductDisliked$!: Observable<boolean>;
  isProductBookmarked$!: Observable<boolean>;
  isProductRated$!: Observable<boolean>;
  authStatus$!: Observable<AuthStatus>;
  productRating$!: Observable<number>;
  productComments$!: Observable<IComment[]>;
  public get AuthStatus() { return AuthStatus; } //for template to use enum

  allRatings = [0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5];
  constructor(private cartService: CartHttpService, 
              private productService: ProductHttpService,
              private _likeProductService: LikeProductHttpService,
              private _bookmarkProductService: BookmarkHttpService,
              private _rateProductService: RateProductHttpService,
              private authService: AuthService,
              private _commentProductService: CommentProductHttpService,
              private route: ActivatedRoute) { 

    this.routeParamsSubscription = this.route.params.subscribe(params => {
      this.productId = params['productId'];
    });

    //ko dùng async nên phải có subcription để unsubscribe khi component destroy
    this.productSubscription = this.productService.paginatedProducts$.pipe(
      map(paginatedProducts => paginatedProducts.products.find(product => product.productId === this.productId)),      
    )
    .subscribe(product => this.product = product);

    this.authStatus$ = this.authService.authStatusGetter$;

    this.isProductLiked$ = this._likeProductService.userLikeProductMappings$.pipe(
      tap((likeProductMappings) => console.log("Init Like product mappings: ", likeProductMappings)),
      map((likeProductMappings) => {
        if (likeProductMappings === null) {
          return false;
        }
        let likeProductWithThisBusinessKey = likeProductMappings.find(
          //enum comparision problem
          //https://stackoverflow.com/questions/39785320/how-to-compare-enums-in-typescript
          // map => map.productBusinessKey === this.product?.businessKey && map.likeStatus === LikeStatus.Liked.valueOf() 
          // this line cause undefined because 
          // key of object is status not likeStatus (not match)
          // we have to change the likeProductMappings interface to match the backend (backend use status)
          map => map.productBusinessKey === this.product?.businessKey && map.status === LikeStatus.Liked.valueOf()
        )
        return likeProductWithThisBusinessKey !== undefined;
      }),
      tap((isLiked) => console.log("Is product liked: ", isLiked))      
    );

    this.isProductDisliked$ = this._likeProductService.userLikeProductMappings$.pipe(
      map((likeProductMappings) => {
        // TODO: rxjs operator to return false skip other operators if condition met
        // if (likeProductMappings === null) {
        //   return false;
        // }
        return likeProductMappings.filter(
          (likeProduct) => likeProduct.status === LikeStatus.Disliked.valueOf()
        )
      }),
      tap((dislikedProducts) => console.log("Dislike product mappings: ", dislikedProducts)),
      map((dislikedProducts) => dislikedProducts.some(
        (dislikedProducts) => dislikedProducts.productBusinessKey === this.product?.businessKey)
      ),
      tap((isDisliked) => console.log("Is product disliked: ", isDisliked))      
    );

    this.isProductBookmarked$ = this._bookmarkProductService.userBookmarkProductMappings$.pipe(
      map((bookmarkProductMappings) => {
        if (bookmarkProductMappings === null) {
          return false;
        }
        return bookmarkProductMappings.some(
          (bookmarkProduct) => bookmarkProduct.productBusinessKey === this.product?.businessKey)
      }),
      tap((isBookmarked) => console.log("Is product bookmarked: ", isBookmarked))      
    );

    this.isProductRated$ = this._rateProductService.userProductRateMappings$.pipe(
      map((userProductRateMappings) => {
        if (userProductRateMappings === null || userProductRateMappings.length === 0) { 
          return false;
        }
        return userProductRateMappings.some(
          (rateMapping) => rateMapping.productBusinessKey === this.product?.businessKey)
      }),
      tap((isRated) => console.log("Is product rated: ", isRated))      
    );

    this.productRating$ = this._rateProductService.userProductRateMappings$.pipe(
      map((userProductRateMappings) => {
        if (userProductRateMappings === null || userProductRateMappings.length === 0) { 
          return 0;
        }
        let ProductRateMappingWithThisBusinessKey = userProductRateMappings.find(
          (rateMapping) => rateMapping.productBusinessKey === this.product?.businessKey
        );
        return ProductRateMappingWithThisBusinessKey?.rating as number;
      }),
      tap((rating) => console.log("Product rating: ", rating))
    )

    this.productComments$ = this._commentProductService.productComments$.pipe(
      map((productComments) => {
        if (productComments === null || productComments.length === 0) {
          return [];
        }
        return productComments;
      }),
      tap((comment) => console.log("Product comment: ", comment))
    )
  }

  ngOnInit(): void {
    this._commentProductService.GetProductComments(this.product?.businessKey as string);
  }
    
  ngOnDestroy(): void {
    this.productSubscription.unsubscribe();
    this.routeParamsSubscription.unsubscribe();
  }

  addModelToCart(event: Event, model: IProductModel) {
    //get the quantity from the input
    event.preventDefault();
    let submitBtn = event.target as HTMLButtonElement;   
    let modelContainer = submitBtn.parentElement?.closest('div') as HTMLDivElement;
    let modelQuantity = modelContainer.querySelector('input[name="quantity"]') as HTMLInputElement;
    
    
    //construct the cartItem to add to cart
    let cartItem = {
      productId: this.product?.productId,
      productModelId: model.productModelId,
      businessKey: this.product?.businessKey,
      quantity: parseInt(modelQuantity.value),
      isOnSale: model.isOnSaleModel,
      saleItemId: model.saleItemId,
      saleType: model.saleType,
      saleValue: model.saleValueModel,
      unitPrice: model.price,
      finalPrice: model.price * parseInt(modelQuantity.value),
      unitAfterSalePrice:  model.isOnSaleModel === false ? undefined : model.priceOnSaleModel,
      finalAfterSalePrice: model.isOnSaleModel === false ? undefined : 
                            model.priceOnSaleModel === undefined ? undefined : 
                            model.priceOnSaleModel * parseInt(modelQuantity.value),      

    };
    //call the service and add to cart
    this.cartService.upsertItemToCart(cartItem);

    //reset the quantity input
    modelQuantity.value = '1';
  }

  likeProduct() {
    let isProductLiked = this._likeProductService.IsProductLiked(this.product?.businessKey as string);
    //if the product is liked, then unlike 
    if (isProductLiked === true) {
      this._likeProductService.unlikeProduct(this.product?.businessKey as string).subscribe(
        (_) => this._likeProductService.GetLikeProductMappings()
      );
      return;
    }
    //else like it
    this._likeProductService.likeProduct(this.product?.businessKey as string).subscribe(
      (_) => this._likeProductService.GetLikeProductMappings()
    );
  }
  
  dislikeProduct() {
    let isProductDisliked = this._likeProductService.IsProductDisliked(this.product?.businessKey as string);
    //if the product is disliked, then undislike
    if (isProductDisliked === true) {
      //unlike is actually undislike too, because it change the status to neutral
      this._likeProductService.unlikeProduct(this.product?.businessKey as string).subscribe(
        (_) => this._likeProductService.GetLikeProductMappings()
      );
      return;
    }
    //else dislike it
    this._likeProductService.dislikeProduct(this.product?.businessKey as string).subscribe(
      (_) => this._likeProductService.GetLikeProductMappings()
    );
  }

  bookmarkProduct() {
    let isProductBookmarked = this._bookmarkProductService.IsProductBookmarked(this.product?.businessKey as string);
    //if the product is bookmarked, then unbookmark
    if (isProductBookmarked === true) {
      this._bookmarkProductService.unBookmarkProduct(this.product?.businessKey as string).subscribe(
        (_) => this._bookmarkProductService.GetBookmarkProductMappings()
      );
      return;
    }
    //else bookmark it
    this._bookmarkProductService.bookmarkProduct(this.product?.businessKey as string).subscribe(
      (_) => this._bookmarkProductService.GetBookmarkProductMappings()
    );
  }

  rateProduct() {
    let ratingInputChecked = document.querySelector('input[name="rating2"]:checked') as HTMLInputElement;
    this._rateProductService.rateProduct(this.product?.businessKey as string, ratingInputChecked.value).subscribe(
      (_) => this._rateProductService.GetUserProductRateMappings()
    );
  }

  commentProduct(commentFormValue: any) {
    this._commentProductService.commentProduct(
      this.authService.userId as string, 
      this.product?.businessKey as string, 
      commentFormValue.commentDetail
    ).subscribe(
      (_) => this._commentProductService.GetProductComments(this.product?.businessKey as string)
    );
  }


}
