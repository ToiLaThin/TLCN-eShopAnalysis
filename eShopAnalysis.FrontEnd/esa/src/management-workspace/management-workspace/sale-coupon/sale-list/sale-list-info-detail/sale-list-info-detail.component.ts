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
  
  hello() {}

  addSaleToModel(evt: Event) {
    //whenever the structure of html change, this has to be changed too
    const clickedBtn = evt.target as HTMLButtonElement;
    const parent = clickedBtn.closest('div') as HTMLDivElement;
    const saleAddBox = parent.querySelector('esa-sale-add') as HTMLElement;
    saleAddBox.style.display = 'block';
  }

  onCloseNotificationBox() {
    (document.querySelector('esa-sale-add') as HTMLElement).style.display = 'none';
  }
}
