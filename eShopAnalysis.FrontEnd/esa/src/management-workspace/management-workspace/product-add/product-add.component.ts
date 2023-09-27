import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { IProduct } from 'src/shared/models/product.interface';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';

@Component({
  selector: 'esa-product-add',
  templateUrl: './product-add.component.html',
  styleUrls: ['./product-add.component.scss']
})
export class ProductAddComponent implements OnInit {

  constructor(private productService: ProductHttpService,
              private router: Router) { }

  newProduct!: IProduct;

  newProductFrmGrp = new FormGroup({
    productName : new FormControl(),
    subCatalogName: new FormControl(),
    productCoverImage: new FormControl(),
  });

  ngOnInit(): void {
    this.newProduct = {
    }
  }

  addProduct() {
    this.newProduct.productName = this.newProductFrmGrp.get('productName')?.value;
    this.newProduct.subCatalogName = this.newProductFrmGrp.get('subCatalogName')?.value;
    this.newProduct.productCoverImage = this.newProductFrmGrp.get('productCoverImage')?.value;
    this.newProduct.productInfo = {
      productDescription: '',
      productBrand: '',
    }
    this.newProduct.productModels = [];
    this.productService.addProduct(this.newProduct).subscribe((product) => {
      this.productService.getProducts()
      .subscribe((products) => {
        //let 's see if this will work => no it's not we must add this
        this.productService.allProductSubject.next(products);
      });
    });
  }

}
