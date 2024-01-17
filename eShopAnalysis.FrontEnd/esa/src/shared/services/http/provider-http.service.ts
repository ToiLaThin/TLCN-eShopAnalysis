import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { IProductModelInfoWithStockAggregate, IProviderRequirement } from 'src/shared/models/provider.interface';
import { environment as env } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class ProviderHttpService {

  allProviderReqSub: BehaviorSubject<IProviderRequirement[]> = new BehaviorSubject<IProviderRequirement[]>([]);
  selectedProviderReqSub: BehaviorSubject<IProviderRequirement | undefined> = new BehaviorSubject<IProviderRequirement | undefined>(undefined);
  allSelectedProviderProductModelInfosWithStockSub: BehaviorSubject<IProductModelInfoWithStockAggregate[]> = new BehaviorSubject<IProductModelInfoWithStockAggregate[]>([]);

  allProviderReq$ = this.allProviderReqSub.asObservable();
  selectedProviderReq$ = this.selectedProviderReqSub.asObservable();
  allSelectedProviderProductModelInfosWithStock$ = this.allSelectedProviderProductModelInfosWithStockSub.asObservable();

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
      }
    );
    console.log(this.selectedProviderReqSub.getValue());
    console.log(this.allSelectedProviderProductModelInfosWithStockSub.getValue());
  }
}
