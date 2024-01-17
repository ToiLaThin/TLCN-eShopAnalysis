import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IProviderRequirement } from 'src/shared/models/provider.interface';
import { ProviderHttpService } from 'src/shared/services/http/provider-http.service';

@Component({
  selector: 'esa-provider-list',
  templateUrl: './provider-list.component.html',
  styleUrls: ['./provider-list.component.scss']
})
export class ProviderListComponent implements OnInit {
  
  constructor(
    private _providerService: ProviderHttpService,
    private route: Router
    ) { }

  allProviderReqs$: Observable<IProviderRequirement[]> = this._providerService.allProviderReq$;
  ngOnInit(): void {
  }

  viewProviderStock(providerRequirementId: string) {
    this._providerService.selectProviderRequirement(providerRequirementId);
    this.route.navigate(['/management/provider-stock/provider-detail']);
  }

}
