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
    // this.getProducts().subscribe((products) => {
    //   this.allProductSubject.next(products);
    // });
    //nhu nay co nghia la khi khoi tao service thi se goi ham getProducts() de lay du lieu
    //1 lan duy nhat
    this.GetProducts();
   }
  ngOnInit(): void {
    
  }

  private getProducts() {
    return this.http.get<IProduct[]>(`${env.BASEURL}/api/ProductCatalog/ProductAPI/GetAllProduct`);
  }
  
  GetProducts() {
    this.getProducts().subscribe((products) => {
      //if return 404, this will not be executed
      console.log(products)
      this.allProductSubject.next(products);
    }, 
    error => {
      // this will log the httpResponse error with status code 404
      console.log(error)
    });
  }
  
  private addProduct(product: IProduct) {
    return this.http.post<IProduct>(`${env.BASEURL}/api/Aggregate/WriteAggregator/AddNewProductAndModelsStock`, product);
  }

  public AddProduct(product: IProduct) {
    this.addProduct(product).subscribe((product) => {
      alert("Add product successfully");
      this.GetProducts();
    });
  }
}
