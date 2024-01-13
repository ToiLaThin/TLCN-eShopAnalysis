import { Component, Input, OnInit } from '@angular/core';
import { IProduct } from 'src/shared/models/product.interface';

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
