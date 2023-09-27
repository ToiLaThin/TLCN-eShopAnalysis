import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { IProduct } from 'src/shared/models/product.interface';
import { environment as env } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductHttpService implements OnInit {

  allProductSubject: BehaviorSubject<IProduct[]> = new BehaviorSubject<IProduct[]>([]);
  allProduct$ = this.allProductSubject.asObservable();

  constructor(private http: HttpClient) {
    this.getProducts().subscribe((products) => {
      this.allProductSubject.next(products);
    });
    //nhu nay co nghia la khi khoi tao service thi se goi ham getProducts() de lay du lieu
    //1 lan duy nhat
   }
  ngOnInit(): void {
    
  }

  getProducts() {
    return this.http.get<IProduct[]>(`${env.BASEURL}/api/ProductAPI/GetAllProduct`);
  }

  addProduct(product: IProduct) {
    return this.http.post<IProduct>(`${env.BASEURL}/api/ProductAPI/AddProduct`, product);
  }
}
