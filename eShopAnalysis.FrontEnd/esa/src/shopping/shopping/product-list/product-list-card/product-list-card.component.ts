import { Component, Input, OnInit } from '@angular/core';
import { IProduct } from 'src/shared/models/product.interface';
import { ICartItem } from 'src/shared/models/cartItem.interface';
import { CartHttpService } from 'src/shared/services/http/cart-http.service';

@Component({
  selector: 'esa-product-list-card',
  templateUrl: './product-list-card.component.html',
  styleUrls: ['./product-list-card.component.scss']
})
export class ProductListCardComponent implements OnInit {

  @Input('product') product!: IProduct;

  constructor() { }

  ngOnInit(): void {
  }

}
