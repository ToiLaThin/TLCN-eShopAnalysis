import { Component, OnInit } from '@angular/core';
import { CatalogHttpService } from 'src/shared/services/http/catalog-http.service';
import { Observable } from 'rxjs';
import { ICatalog } from 'src/shared/models/catalog.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'esa-catalog-list',
  templateUrl: './catalog-list.component.html',
  styleUrls: ['./catalog-list.component.scss']
})
export class CatalogListComponent implements OnInit {
  allCatalogs$! : Observable<ICatalog[]>;
  displayedColumns = ['catalogName', 'catalogDescription'];
  constructor(private catalogService: CatalogHttpService,
              private router: Router) {
    this.allCatalogs$ = this.catalogService.allCatalog$;
    this.catalogService.GetAllCatalogs();
  }

  ngOnInit(): void {
  }

  addSubCatalog(catalogId?: string) {
    // this.catalogService.GetAllSubCatalogs(catalogId);
  }

  viewSubCatalog(catalogId?: string) {
    this.router.navigateByUrl(
      `/management/product-catalog/subcatalog-list/${catalogId}`, 
      { replaceUrl: true}
    );
  }
}
