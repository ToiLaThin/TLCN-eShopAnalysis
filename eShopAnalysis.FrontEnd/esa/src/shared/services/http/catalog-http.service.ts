import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment as env } from 'src/environments/environment';
import { ICatalog } from 'src/shared/models/catalog.interface';
import { ISubCatalog } from 'src/shared/models/subcatalog.interface';

@Injectable({
  providedIn: 'root'
})
export class CatalogHttpService {

  allCatalogSubject: BehaviorSubject<ICatalog[]> = new BehaviorSubject<ICatalog[]>([]);
  allCatalog$ = this.allCatalogSubject.asObservable();
  get currentCatalogsValue(): ICatalog[] {
    return this.allCatalogSubject.getValue();
  }
  allSubCatalogSubject: BehaviorSubject<ISubCatalog[]> = new BehaviorSubject<ISubCatalog[]>([]);
  allSubCatalog$ = this.allSubCatalogSubject.asObservable();
  constructor(private http: HttpClient) { 
    this.GetAllCatalogs();
  }

  private getAllCatalogs() {
    return this.http.get<ICatalog[]>(`${env.BASEURL}/api/CatalogAPI/GetAllCatalog`);
  }

  public GetAllCatalogs() {
    this.getAllCatalogs().subscribe((catalogs) => {
      this.allCatalogSubject.next(catalogs);
    });
  }
  private getAllSubCatalogs(catalogId: string) {
    let httpHeaders = new HttpHeaders();
    httpHeaders = httpHeaders.append('catalogId', catalogId);
    return this.http.get<ISubCatalog[]>(`${env.BASEURL}/api/CatalogAPI/GetAllSubCatalogs`, { headers: httpHeaders });
  }

  public GetAllSubCatalogs(catalogId: string) {
    this.getAllSubCatalogs(catalogId).subscribe((subcatalogs) => {
      this.allSubCatalogSubject.next(subcatalogs);
    });
  }

  private addCatalog(catalog: ICatalog) {
    return this.http.post<ICatalog>(`${env.BASEURL}/api/CatalogAPI/CreateCatalog`, catalog);
  }

  public AddCatalog(catalog: ICatalog) {
    this.addCatalog(catalog).subscribe((catalog) => {
      this.GetAllCatalogs();
    });
  }

  private addSubCatalog(subCatalog: ISubCatalog, catalogId: string) {
    let httpHeaders = new HttpHeaders();
    httpHeaders = httpHeaders.append('catalogId', catalogId);
    return this.http.post<ISubCatalog>(`${env.BASEURL}/api/CatalogAPI/CreateSubCatalog`, subCatalog, { headers: httpHeaders });
  }

  public AddSubCatalog(subCatalog: ISubCatalog, catalogId: string) {
    this.addSubCatalog(subCatalog, catalogId).subscribe((subCatalog) => {
      this.GetAllSubCatalogs(catalogId);
    });
  }


}
