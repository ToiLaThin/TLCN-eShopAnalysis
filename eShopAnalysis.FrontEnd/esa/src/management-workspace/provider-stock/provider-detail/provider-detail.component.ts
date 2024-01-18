import { Component, OnInit } from '@angular/core';
import { combineLatest, map, tap } from 'rxjs';
import { IProductModelInfoMergeStockItemRequest } from 'src/shared/models/provider.interface';
import { ProviderHttpService } from 'src/shared/services/http/provider-http.service';

@Component({
  selector: 'esa-provider-detail',
  templateUrl: './provider-detail.component.html',
  styleUrls: ['./provider-detail.component.scss']
})
export class ProviderDetailComponent implements OnInit {

  constructor(private _providerService: ProviderHttpService) { }

  selectedProviderReq$ = this._providerService.selectedProviderReq$;
  //allSelectedProviderProductModelInfosWithStock$ = this._providerService.allSelectedProviderProductModelInfosWithStock$;
  allProductModelInfoMergeStockItemReqs$ = this._providerService.allProductModelInfoMergeStockItemReqs$

  ngOnInit(): void {
  }

  increaseStockQuantity(productModelId: string) {
    this._providerService.IncreaseStockRequestQuantity(productModelId);
    
  }

  decreaseStockQuantity(productModelId: string) {
    this._providerService.DecreaseStockRequestQuantity(productModelId);    
  }

  confirmRequestToProvider() {
    this._providerService.ConfirmRequestToProvider();
  }

}
