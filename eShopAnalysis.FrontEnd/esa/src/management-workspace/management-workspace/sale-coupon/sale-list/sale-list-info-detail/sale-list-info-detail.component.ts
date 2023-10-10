import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { IProduct } from 'src/shared/models/product.interface';

@Component({
  selector: 'esa-sale-list-info-detail',
  templateUrl: './sale-list-info-detail.component.html',
  styleUrls: ['./sale-list-info-detail.component.scss']
})
export class SaleListInfoDetailComponent implements OnInit  {

  constructor() { }
  @Input('selectedProduct') product!: IProduct | null;
  ngOnInit(): void {
  }
  isAddingSaleToModel: boolean = false;
  hello() {}

  addSaleToModel() {
    console.log("addSaleToModel");
    
    this.isAddingSaleToModel = true;
  }

  onCloseNotificationBox() {
    this.isAddingSaleToModel = false;
  }
}
