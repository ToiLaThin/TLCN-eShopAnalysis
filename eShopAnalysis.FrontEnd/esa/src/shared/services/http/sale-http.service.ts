import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ISaleItem } from 'src/shared/models/saleItem.interface';
import { environment as env } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SaleHttpService {

  constructor(private http: HttpClient) { }

  private addSaleToModel(saleItem: ISaleItem) {
    return this.http.post<ISaleItem>(`${env.BASEURL}/api/Aggregate/WriteAggregator/AddSaleItemAndUpdateProductToOnSale`, saleItem);
  }

  public AddSaleToModel(saleItem: ISaleItem) {
    this.addSaleToModel(saleItem).subscribe((saleItem) => {      
      console.log("AddSaleToModel"); 
      //reload the sale list to see the changes     
    });
  }
}
