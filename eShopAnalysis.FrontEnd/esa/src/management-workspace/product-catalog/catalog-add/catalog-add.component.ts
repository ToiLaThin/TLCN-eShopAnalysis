import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ICatalog } from 'src/shared/models/catalog.interface';
import { CatalogHttpService } from 'src/shared/services/http/catalog-http.service';

@Component({
  selector: 'esa-catalog-add',
  templateUrl: './catalog-add.component.html',
  styleUrls: ['./catalog-add.component.scss']
})
export class CatalogAddComponent implements OnInit {

  constructor(private catalogService: CatalogHttpService,
              private router: Router) {
    
  }

  ngOnInit(): void {
    this.newCatalog = { //these two are required
      catalogName: '',
      catalogDescription: '',
    }
  }

  newCatalog!: ICatalog;

  newCatalogFrmGrp = new FormGroup({
    catalogName: new FormControl(),
    catalogDescription: new FormControl(),
    catalogImage: new FormControl(),
  });

  addCatalog() {
    this.newCatalog.catalogName = this.newCatalogFrmGrp.value.catalogName;
    this.newCatalog.catalogDescription = this.newCatalogFrmGrp.value.catalogDescription;
    this.newCatalog.catalogImage = this.newCatalogFrmGrp.value.catalogImage;
    this.newCatalog.subCatalogs = [];
    this.catalogService.AddCatalog(this.newCatalog);
  }

}
