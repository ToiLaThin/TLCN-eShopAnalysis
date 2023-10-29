import { Component, OnInit } from '@angular/core';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';
import { Observable, Subscription } from 'rxjs';
import { IProduct } from 'src/shared/models/product.interface';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'esa-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {

  allProduct$! : Observable<IProduct[]>;
  private _subcription!: Subscription;
  constructor(private productService: ProductHttpService,private route: Router) {
    this.allProduct$ = this.productService.allProduct$;    
  }

  ngOnInit(): void {
  }

  viewProductDetail(productId: string | undefined) {
    this.route.navigate(['shopping', 'product-detail', productId]);
  }
}
