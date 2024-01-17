import { Component, OnInit } from '@angular/core';
import { ProviderHttpService } from 'src/shared/services/http/provider-http.service';

@Component({
  selector: 'esa-provider-detail',
  templateUrl: './provider-detail.component.html',
  styleUrls: ['./provider-detail.component.scss']
})
export class ProviderDetailComponent implements OnInit {

  constructor(private _providerService: ProviderHttpService) { }

  selectedProviderReq$ = this._providerService.selectedProviderReq$;
  allSelectedProviderProductModelInfosWithStock$ = this._providerService.allSelectedProviderProductModelInfosWithStock$;
  ngOnInit(): void {
  }

}
