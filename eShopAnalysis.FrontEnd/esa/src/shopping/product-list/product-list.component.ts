import { Component, OnInit } from '@angular/core';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';
import { Observable, Subscription, map } from 'rxjs';
import { FilterBy, IProduct, IProductLazyLoadRequest, OrderType, ProductPerPage, SortBy } from 'src/shared/models/product.interface';
import { ActivatedRoute, Router } from '@angular/router';
import { ICatalog, ISubCatalog } from 'src/shared/models/catalog.interface';
import { CatalogHttpService } from 'src/shared/services/http/catalog-http.service';

@Component({
  selector: 'esa-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {

  fromPrice: number = 0;
  toPrice: number = 10000000;
  allToDisplayProduct$! : Observable<IProduct[]>;
  allCatalog$! : Observable<ICatalog[]>;
  selectedSubCatalogs$! : Observable<ISubCatalog[]>;
  totalPage$! : Observable<number>;
  totalPageAsArray$! : Observable<number[]>; 
  
  numProductPerPageEnums = Object.keys(ProductPerPage).filter(k => !isNaN(parseInt(k))).map(k => ({
    key: k, 
    value: k
  }));
  sortByEnums = Object.keys(SortBy).filter(k => !isNaN(parseInt(k))).map(k => ({
    key: k, 
    value: SortBy[k as any]
  }));
  orderTypeEnums = Object.keys(OrderType).filter(k => !isNaN(parseInt(k))).map(k => ({
    key: k, 
    value: OrderType[k as any]
  }));
  
  private _subcription!: Subscription;
  constructor(private productService: ProductHttpService, 
              private catalogService:CatalogHttpService,
              private route: Router) {

    this.allToDisplayProduct$ = this.productService.paginatedProducts$.pipe(
      map(paginatedProducts => paginatedProducts.products)
    );    
    this.allCatalog$ = this.catalogService.allCatalog$;
    this.selectedSubCatalogs$ = this.catalogService.allSubCatalog$;
    this.totalPage$ = this.productService.paginatedProducts$.pipe(
      map(paginatedProducts => paginatedProducts.pageCount)
    );
    this.totalPageAsArray$ = this.totalPage$.pipe(
      map(totalPage => Array(totalPage).fill(1).map((x,i)=>i + 1))
    ); 
  }

  ngOnInit(): void {
  }

  changePriceRange() {
    const fromPrice: number = this.fromPrice;
    const toPrice: number = this.toPrice;
    if (fromPrice > toPrice)  {
      alert("Please enter valid price range");
      return;
    }
    const priceMeta: string = JSON.stringify({
      fromPrice: fromPrice,
      toPrice: toPrice
    });
    const oldRequest:IProductLazyLoadRequest = this.productService.lazyLoadRequestSubject.value;
    const filteredByPriceRange = this.productService.lazyLoadRequestSubject.value.filterRequests.some((filterRequest) => filterRequest.filterBy == FilterBy.Price);
    if (filteredByPriceRange === true) {
      console.log("filteredByPriceRange is already true");
      
      const indexOldFilterRequest = this.productService.lazyLoadRequestSubject.value.filterRequests.findIndex((filterRequest) => filterRequest.filterBy == FilterBy.Price);
      this.productService.lazyLoadRequestSubject.next({
        ...oldRequest,
        filterRequests: [
          ...oldRequest.filterRequests.slice(0, indexOldFilterRequest),
          {
            filterBy: FilterBy.Price,
            Meta: priceMeta
          },
          ...oldRequest.filterRequests.slice(indexOldFilterRequest + 1)
        ]
      });
      this.productService.GetProducts();
      return;
    }
    
    console.log("filteredByPriceRange is false");
    this.productService.lazyLoadRequestSubject.next({
      ...oldRequest,
      filterRequests: [
        ...oldRequest.filterRequests,
        {
          filterBy: FilterBy.Price,
          Meta: priceMeta
        }
      ]
    });
    this.productService.GetProducts();
  }

  changePage(pageNumber: number) {
    const oldRequest:IProductLazyLoadRequest = this.productService.lazyLoadRequestSubject.value;
    this.productService.lazyLoadRequestSubject.next({
      ...oldRequest,
      pageOffset: pageNumber
    });
    this.productService.GetProducts();
  }

  changeOrderType(target: EventTarget | null) {
    const selectedElement: HTMLInputElement = (target as HTMLInputElement);
    const orderType: OrderType = parseInt(selectedElement.value);
    const oldRequest:IProductLazyLoadRequest = this.productService.lazyLoadRequestSubject.value;
    this.productService.lazyLoadRequestSubject.next({
      ...oldRequest,
      orderType: orderType
    });
    this.productService.GetProducts();
  }

  changeSortBy(target: EventTarget | null) {
    const selectedElement: HTMLInputElement = (target as HTMLInputElement);
    const sortBy: SortBy = parseInt(selectedElement.value);
    const oldRequest:IProductLazyLoadRequest = this.productService.lazyLoadRequestSubject.value;
    this.productService.lazyLoadRequestSubject.next({
      ...oldRequest,
      sortBy: sortBy
    });
    this.productService.GetProducts();
  }

  changeProductPerPage(target: EventTarget | null) {
    const selectedElement: HTMLInputElement = (target as HTMLInputElement);
    const productPerPage: ProductPerPage = parseInt(selectedElement.value);
    const oldRequest:IProductLazyLoadRequest = this.productService.lazyLoadRequestSubject.value;
    this.productService.lazyLoadRequestSubject.next({
      ...oldRequest,
      productPerPage: productPerPage
    });
    this.productService.GetProducts();
  }

  onCatalogChange() {
    let catalogId = (document.getElementById('catalog') as HTMLSelectElement).value;
    this.catalogService.GetAllSubCatalogs(catalogId);
  }

  private addSub(subCatalogId: string) {
    const filteredBySub = this.productService.lazyLoadRequestSubject.value.filterRequests.find((filterRequest) => filterRequest.filterBy == 0);
    if(!filteredBySub) {
      this.productService.lazyLoadRequestSubject.value.filterRequests.push({
        filterBy: 0,
        Meta: JSON.stringify([subCatalogId])
      });
      return;
    }
    const oldRequest:IProductLazyLoadRequest = this.productService.lazyLoadRequestSubject.value;
    const oldFilterRequest = oldRequest.filterRequests.filter((filterRequest) => filterRequest.filterBy == 0)[0];
    const indexOldFilterRequest = oldRequest.filterRequests.findIndex((filterRequest) => filterRequest.filterBy == 0);
    let newFilterRequest = {
      filterBy: oldFilterRequest.filterBy,
      Meta: JSON.stringify(
        JSON.parse(oldFilterRequest.Meta).concat([subCatalogId])
      )
    };
    
    this.productService.lazyLoadRequestSubject.next({
      ...oldRequest,
      filterRequests: [
        ...oldRequest.filterRequests.slice(0, indexOldFilterRequest),
        newFilterRequest,
        ...oldRequest.filterRequests.slice(indexOldFilterRequest + 1)
      ]
    });
  }

  private removeSub(subCatalogId: string) {
    const oldRequest:IProductLazyLoadRequest = this.productService.lazyLoadRequestSubject.value;
    const oldFilterRequest = oldRequest.filterRequests.filter((filterRequest) => filterRequest.filterBy == 0)[0];
    const indexOldFilterRequest = oldRequest.filterRequests.findIndex((filterRequest) => filterRequest.filterBy == 0);

    if (JSON.parse(oldFilterRequest.Meta).length === 1) {
      //remove filter request of subcatalog, not just remove subcatalogId in Meta
      this.productService.lazyLoadRequestSubject.next({
        ...oldRequest,        
        filterRequests: [
          ...oldRequest.filterRequests.slice(0, indexOldFilterRequest),
          ...oldRequest.filterRequests.slice(indexOldFilterRequest + 1)
        ]
      });
      return;
    }

    let newFilterRequest = {
      filterBy: oldFilterRequest.filterBy,
      Meta: JSON.stringify(
        JSON.parse(oldFilterRequest.Meta).filter((subId: string) => subId != subCatalogId)
      )
    };
    this.productService.lazyLoadRequestSubject.next({
      ...oldRequest,
      filterRequests: [
        ...oldRequest.filterRequests.slice(0, indexOldFilterRequest),
        newFilterRequest,
        ...oldRequest.filterRequests.slice(indexOldFilterRequest + 1)
      ]
    });
  }

  selectSubCatalog(select: EventTarget | null) {
    const selectEle: HTMLInputElement = (select as HTMLInputElement);
    const subCatalogId: string = selectEle.value;
    if (selectEle.checked === true) {
      this.addSub(subCatalogId);
      this.productService.GetProducts();
      return;
    }
    this.removeSub(subCatalogId)
    this.productService.GetProducts();
  }

  viewProductDetail(productId: string | undefined) {
    this.route.navigate(['shopping', 'product-detail', productId]);
  }
}
