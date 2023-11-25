import { Component, OnDestroy, OnInit } from '@angular/core';
import { CartHttpService } from 'src/shared/services/http/cart-http.service';
import { IProduct, IProductModel } from 'src/shared/models/product.interface';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';
import { Subscription, map } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'esa-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit, OnDestroy {

  constructor(private cartService: CartHttpService, 
              private productService: ProductHttpService,
              private route: ActivatedRoute) { }
  productSubscription!: Subscription;
  routeParamsSubscription!: Subscription;
  productId!: string;
  product!: IProduct | undefined;
  ngOnInit(): void {
    this.routeParamsSubscription = this.route.params.subscribe(params => {
      this.productId = params['productId'];
    });
    this.productSubscription = this.productService.paginatedProducts$.pipe(
      map(paginatedProducts => paginatedProducts.products.find(product => product.productId === this.productId)),      
      )
      .subscribe(product => this.product = product);
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
  

}
