import { Component, OnInit } from '@angular/core';
import { ProductHttpService } from 'src/shared/services/http/product-http.service';
import { Observable, Subscription } from 'rxjs';
import { IProduct, IProductLazyLoadRequest, OrderType, ProductPerPage, SortBy } from 'src/shared/models/product.interface';
import { ActivatedRoute, Router } from '@angular/router';
import { ICatalog, ISubCatalog } from 'src/shared/models/catalog.interface';
import { CatalogHttpService } from 'src/shared/services/http/catalog-http.service';

@Component({
  selector: 'esa-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {

  allProduct$! : Observable<IProduct[]>;
  allCatalog$! : Observable<ICatalog[]>;
  selectedSubCatalogs$! : Observable<ISubCatalog[]>;

  
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
    this.allProduct$ = this.productService.allProduct$;    
    this.allCatalog$ = this.catalogService.allCatalog$;
    this.selectedSubCatalogs$ = this.catalogService.allSubCatalog$;
  }

  ngOnInit(): void {
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
