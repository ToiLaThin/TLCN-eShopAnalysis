import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { IProductModelInfoMergeStockItemRequest, IProductModelInfoWithStockAggregate, IProviderRequirement, IStockItemRequest, IStockRequestTransaction } from 'src/shared/models/provider.interface';
import { environment as env } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class ProviderHttpService {
  //service become too complex
  allProviderReqSub: BehaviorSubject<IProviderRequirement[]> = new BehaviorSubject<IProviderRequirement[]>([]);
  selectedProviderReqSub: BehaviorSubject<IProviderRequirement | undefined> = new BehaviorSubject<IProviderRequirement | undefined>(undefined);
  //all get from aggregate read controller server-side
  allSelectedProviderProductModelInfosWithStockSub: BehaviorSubject<IProductModelInfoWithStockAggregate[]> = new BehaviorSubject<IProductModelInfoWithStockAggregate[]>([]);
  //map from IProductModelInfoWithStockAggregate to model that have more prop for display is IProductModelInfoMergeStockItemRequest, these thing actually can do in backend
  allProductModelInfoMergeStockItemReqsSub: BehaviorSubject<IProductModelInfoMergeStockItemRequest[]> = new BehaviorSubject<IProductModelInfoMergeStockItemRequest[]>([])

  allProviderReq$ = this.allProviderReqSub.asObservable();
  selectedProviderReq$ = this.selectedProviderReqSub.asObservable();
  allSelectedProviderProductModelInfosWithStock$ = this.allSelectedProviderProductModelInfosWithStockSub.asObservable();
  allProductModelInfoMergeStockItemReqs$ = this.allProductModelInfoMergeStockItemReqsSub.asObservable();

  constructor(private http: HttpClient) { 
    //already authenticated because of guard
    this.getAllProviderRequirements().subscribe(
      providerReqs => {
        this.allProviderReqSub.next(providerReqs);
      }
    );
  }

  public getAllProviderRequirements(): Observable<IProviderRequirement[]> {
    return this.http.get<IProviderRequirement[]>(`${env.BASEURL}/api/StockProviderRequest/ProviderRequirementAPI/GetAllProviderRequirements`);    
  }

  public getProductModelInfosWithStockOfProvider()
  {
    let providerRequirementSelected: IProviderRequirement = this.selectedProviderReqSub.getValue() as IProviderRequirement;
    return this.http.post<IProductModelInfoWithStockAggregate[]>(`${env.BASEURL}/api/Aggregate/ReadAggregator/GetProductModelInfosWithStockOfProvider`, providerRequirementSelected.availableStockItemRequestMetas);
  }

  public selectProviderRequirement(providerRequirementId: string) {
    this.selectedProviderReqSub.next(
      this.allProviderReqSub.getValue().find(req => req.providerRequirementId === providerRequirementId)
    );
    this.getProductModelInfosWithStockOfProvider().subscribe(
      productModelInfos => {
        this.allSelectedProviderProductModelInfosWithStockSub.next(productModelInfos);
        let allSelectedProviderProductModelInfosWithStock = this.allSelectedProviderProductModelInfosWithStockSub.getValue();
        let allProductModelInfoMergeStockItemReqs = allSelectedProviderProductModelInfosWithStock.map(productModelInfo => {
          //INIT state
          let productModelInfoMergeStockItemReq = {
            productId: productModelInfo.productId,
            productModelId: productModelInfo.productModelId,
            businessKey: productModelInfo.businessKey,
            productModelName: productModelInfo.productModelName,
            productCoverImage: productModelInfo.productCoverImage,
            price: productModelInfo.price,
            currentQuantity: productModelInfo.currentQuantity, //current quantity we are having in stock, not the number we request
            unitRequestPrice: productModelInfo.unitRequestPrice,          
            itemQuantity: 0, //the number we request
            totalItemRequestPrice: 0,
            afterRequestQuantity: productModelInfo.currentQuantity
            
          } as IProductModelInfoMergeStockItemRequest;
          return productModelInfoMergeStockItemReq;
        })
        this.allProductModelInfoMergeStockItemReqsSub.next(allProductModelInfoMergeStockItemReqs);

        //the anonymous func is executed after
        console.log("Selected Provider Product Model Info with Stock:", this.allSelectedProviderProductModelInfosWithStockSub.getValue());
        console.log("All Product Model Info merged with Stock Item Request:", this.allProductModelInfoMergeStockItemReqsSub.getValue())
      }
    );
    console.log("Selected Provider Request:", this.selectedProviderReqSub.getValue());
    
  }

  public DecreaseStockRequestQuantity(productModelId: string) {
    let allProductModelInfoMergeStockItemReqs = this.allProductModelInfoMergeStockItemReqsSub.getValue();
    let mergedItem = allProductModelInfoMergeStockItemReqs.filter((mergedItem) => mergedItem.productModelId === productModelId);
    if (mergedItem.length !== 1) { alert("Something is wrong with the product model Id") } //only accept if not product model id duplicated
    console.log("Quantity before decrease:", mergedItem[0].itemQuantity);
    mergedItem[0].itemQuantity = mergedItem[0].itemQuantity - 1;
    mergedItem[0].totalItemRequestPrice = mergedItem[0].unitRequestPrice * mergedItem[0].itemQuantity;
    mergedItem[0].afterRequestQuantity = mergedItem[0].currentQuantity + mergedItem[0].itemQuantity
    this.allProductModelInfoMergeStockItemReqsSub.next(allProductModelInfoMergeStockItemReqs);
    // tested
    console.log("Quantity after decrease:", mergedItem[0].itemQuantity);
    console.log(this.allProductModelInfoMergeStockItemReqsSub.getValue())
  }

  public IncreaseStockRequestQuantity(productModelId: string) {
    let allProductModelInfoMergeStockItemReqs = this.allProductModelInfoMergeStockItemReqsSub.getValue();
    let mergedItem = allProductModelInfoMergeStockItemReqs.filter((mergedItem) => mergedItem.productModelId === productModelId);
    if (mergedItem.length !== 1) { alert("Something is wrong with the product model Id") }
    console.log("Quantity before increase:", mergedItem[0].itemQuantity);
    mergedItem[0].itemQuantity = mergedItem[0].itemQuantity + 1;
    mergedItem[0].totalItemRequestPrice = mergedItem[0].unitRequestPrice * mergedItem[0].itemQuantity;
    mergedItem[0].afterRequestQuantity = mergedItem[0].currentQuantity + mergedItem[0].itemQuantity
    this.allProductModelInfoMergeStockItemReqsSub.next(allProductModelInfoMergeStockItemReqs);
    // tested
    console.log("Quantity after increase:", mergedItem[0].itemQuantity);
    console.log(this.allProductModelInfoMergeStockItemReqsSub.getValue())

  }

  ConfirmRequestToProvider() {
    let selectedProviderReq = this.selectedProviderReqSub.getValue();
    let allStockItemReqs = this.allProductModelInfoMergeStockItemReqsSub.getValue()
    .filter(mergedItem => mergedItem.itemQuantity > 0)
    .map(mergedItem => {
      let stockItemReq = {
        productId: mergedItem.productId,
        productModelId: mergedItem.productModelId,
        businessKey: mergedItem.businessKey,
        unitRequestPrice: mergedItem.unitRequestPrice,
        itemQuantity: mergedItem.itemQuantity,
        totalItemRequestPrice: mergedItem.totalItemRequestPrice
      } as IStockItemRequest;
      return stockItemReq;
    });
    if (selectedProviderReq === undefined || allStockItemReqs === undefined) { 
      alert("Please select a provider requirement && request items") 
    }
    let requestBody: IStockRequestTransaction = {
      //stockRequestTransactionId is not needed to be supplied, it will be generated in db
      providerRequirementId: selectedProviderReq?.providerRequirementId as string,
      providerBusinessKey: selectedProviderReq?.providerBusinessKey as string,
      //these two can be done in backend
      // totalTransactionPrice: allStockItemReqs.reduce(
      //   (total, item) => total + item.totalItemRequestPrice
      // , 0),
      // totalQuantity: allStockItemReqs.reduce(
      //   (total, item) => total + item.itemQuantity
      // , 0),
      // dateCreated: new Date(),
      stockItemRequests: allStockItemReqs
    }
    console.log(requestBody);
    this.http.post(`${env.BASEURL}/api/Aggregate/WriteAggregator/AddStockReqTransAndIncreaseStockItems`, requestBody).subscribe(
      //reset all ui model observable
      (_) => {
        this.getProductModelInfosWithStockOfProvider().subscribe(
          productModelInfos => {
            this.allSelectedProviderProductModelInfosWithStockSub.next(productModelInfos);
            let allSelectedProviderProductModelInfosWithStock = this.allSelectedProviderProductModelInfosWithStockSub.getValue();
            let allProductModelInfoMergeStockItemReqs = allSelectedProviderProductModelInfosWithStock.map(productModelInfo => {
              //INIT state
              let productModelInfoMergeStockItemReq = {
                productId: productModelInfo.productId,
                productModelId: productModelInfo.productModelId,
                businessKey: productModelInfo.businessKey,
                productModelName: productModelInfo.productModelName,
                productCoverImage: productModelInfo.productCoverImage,
                price: productModelInfo.price,
                currentQuantity: productModelInfo.currentQuantity, //current quantity we are having in stock, not the number we request
                unitRequestPrice: productModelInfo.unitRequestPrice,          
                itemQuantity: 0, //the number we request
                totalItemRequestPrice: 0,
                afterRequestQuantity: productModelInfo.currentQuantity
                
              } as IProductModelInfoMergeStockItemRequest;
              return productModelInfoMergeStockItemReq;
            })
            this.allProductModelInfoMergeStockItemReqsSub.next(allProductModelInfoMergeStockItemReqs);
    
            //the anonymous func is executed after
            console.log("Selected Provider Product Model Info with Stock:", this.allSelectedProviderProductModelInfosWithStockSub.getValue());
            console.log("All Product Model Info merged with Stock Item Request:", this.allProductModelInfoMergeStockItemReqsSub.getValue())
          }
        );
      }
    );
  }

}
