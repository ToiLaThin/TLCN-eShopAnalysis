import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { CatalogHttpService } from 'src/shared/services/http/catalog-http.service';
import { Observable } from 'rxjs';
import { ICatalog, ISubCatalog } from 'src/shared/models/catalog.interface';

@Component({
  selector: 'esa-subcatalog-add',
  templateUrl: './subcatalog-add.component.html',
  styleUrls: ['./subcatalog-add.component.scss']
})
export class SubcatalogAddComponent implements OnInit {

  constructor(private catalogService: CatalogHttpService,
              private router: Router) { }

  ngOnInit(): void {
    this.allCatalogs$ = this.catalogService.allCatalog$;
    this.newSubCatalog = {
      subCatalogName: '',
      subCatalogDescription: '',
    }
  }

  allCatalogs$!: Observable<ICatalog[]>;
  newSubCatalog!: ISubCatalog;

  newSubCatalogFrmGrp = new FormGroup({
    selectedCatalogId: new FormControl(),
    subCatalogName: new FormControl(),
    subCatalogDescription: new FormControl(),
    subCatalogImage: new FormControl(),
  })

  addSubCatalog() {
    this.newSubCatalog.subCatalogName = this.newSubCatalogFrmGrp.value.subCatalogName;
    this.newSubCatalog.subCatalogDescription = this.newSubCatalogFrmGrp.value.subCatalogDescription;
    this.newSubCatalog.subCatalogImage = this.newSubCatalogFrmGrp.value.subCatalogImage;

    const selectedCatalogId = this.newSubCatalogFrmGrp.value.selectedCatalogId;
    this.catalogService.AddSubCatalog(this.newSubCatalog, selectedCatalogId);
  }
}
