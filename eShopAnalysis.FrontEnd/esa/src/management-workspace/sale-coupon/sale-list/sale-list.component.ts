import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Observable, map } from 'rxjs';
import { IProduct } from 'src/shared/models/product.interface';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';
import { SaleListInfoDetailComponent } from './sale-list-info-detail/sale-list-info-detail.component';

@Component({
  selector: 'esa-sale-list',
  templateUrl: './sale-list.component.html',
  styleUrls: ['./sale-list.component.scss']
})
export class SaleListComponent implements OnInit,AfterViewInit  {

  allProduct$!: Observable<IProduct[]>;
  displayedColumns: string[] = ['name', 'isOnSale'];
  @ViewChild('productSaleDetail') productSaleDetail!: SaleListInfoDetailComponent;
  constructor(private productService: ProductHttpService) { }  
  selectedProduct!: IProduct | null;

  ngOnInit(): void {
    this.allProduct$ = this.productService.paginatedProducts$.pipe(
      map(paginatedProducts => paginatedProducts.products)
    );
    this.selectedProduct = null;
  }

  ngAfterViewInit(): void {
    this.productSaleDetail.product = this.selectedProduct;
  }

  displayThisProduct(product: IProduct) {
    console.log(product);    
    this.selectedProduct = product; //automaticall pass to child component
  }

}
