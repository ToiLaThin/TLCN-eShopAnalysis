import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CatalogHttpService } from 'src/shared/services/http/catalog-http.service';

@Component({
  selector: 'esa-subcatalog-list',
  templateUrl: './subcatalog-list.component.html',
  styleUrls: ['./subcatalog-list.component.scss']
})
export class SubcatalogListComponent implements OnInit {

  constructor(private route: ActivatedRoute,
              private catalogService: CatalogHttpService) { }

  catalogId: string = '';
  catalogName: string = '';
  allSubCatalogs$ = this.catalogService.allSubCatalog$;
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.catalogId = params['catalogId'];
      this.catalogName = this.catalogService.currentCatalogsValue.find((catalog) => catalog.catalogId === this.catalogId)?.catalogName || 'Cound not find catalog name';
      this.catalogService.GetAllSubCatalogs(this.catalogId);
    })
  }

}
