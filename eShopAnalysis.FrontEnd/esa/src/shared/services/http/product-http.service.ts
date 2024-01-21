import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { IPaginatedProduct, IProduct, IProductLazyLoadRequest, OrderType, ProductPerPage, SortBy } from 'src/shared/models/product.interface';
import { environment as env } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class ProductHttpService implements OnInit {

  lazyLoadRequestSubject!: BehaviorSubject<IProductLazyLoadRequest>;
  paginatedProductsSub: BehaviorSubject<IPaginatedProduct> = new BehaviorSubject<IPaginatedProduct>({} as IPaginatedProduct);
  paginatedProducts$: Observable<IPaginatedProduct> = this.paginatedProductsSub.asObservable();  

  constructor(private http: HttpClient) {
    // this.getProducts().subscribe((products) => {
    //   this.allProductSubject.next(products);
    // });
    //nhu nay co nghia la khi khoi tao service thi se goi ham getProducts() de lay du lieu
    //1 lan duy nhat
    //init request
    this.lazyLoadRequestSubject = new BehaviorSubject<IProductLazyLoadRequest>({
      pageOffset: 1,
      productPerPage: ProductPerPage.Sixteen,
      sortBy: SortBy.Id,
      orderType: OrderType.Ascending,
      filterRequests: []
    });
    this.GetProducts();
   }
  ngOnInit(): void {
    
  }

  private getProducts() {
    // return this.http.get<IProduct[]>(`${env.BASEURL}/api/ProductCatalog/ProductAPI/GetAllProduct`);
    return this.http.post<IPaginatedProduct>(`${env.BASEURL}/api/ProductCatalog/ProductAPI/GetProductsLazyLoad`, this.lazyLoadRequestSubject.value);
  }
  
  GetProducts() {
    this.getProducts().subscribe((paginatedProducts) => {
      //if return 404, this will not be executed
      this.paginatedProductsSub.next(paginatedProducts);
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
    this.addProduct(product).subscribe((_) => {
      alert("Add product successfully");
      this.GetProducts();
    });
  }
}
