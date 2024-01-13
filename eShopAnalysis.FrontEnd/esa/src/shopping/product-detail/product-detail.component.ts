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
  authStatus$!: Observable<AuthStatus>;
  public get AuthStatus() { return AuthStatus; } //for template to use enum

  constructor(private cartService: CartHttpService, 
              private productService: ProductHttpService,
              private _likeProductService: LikeProductHttpService,
              private authService: AuthService,
              private route: ActivatedRoute) { 

    this.routeParamsSubscription = this.route.params.subscribe(params => {
      this.productId = params['productId'];
    });

    this.productSubscription = this.productService.paginatedProducts$.pipe(
      map(paginatedProducts => paginatedProducts.products.find(product => product.productId === this.productId)),      
    )
    .subscribe(product => this.product = product);

    this.authStatus$ = this.authService.authStatusGetter$;

    this.isProductLiked$ = this._likeProductService.userLikeProductMappings$.pipe(
      tap((likeProductMappings) => console.log("Init Like product mappings: ", likeProductMappings)),
      map((likeProductMappings) => {
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
  }

  ngOnInit(): void {    
    
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

}
