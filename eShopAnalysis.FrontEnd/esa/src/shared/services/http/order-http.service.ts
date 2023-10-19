import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ICustomerOrderInfoConfirmedRequest } from 'src/shared/models/customerOrderInfo.interface';
import { AuthService } from '../auth.service';
import { IOrderDraftViewModel } from 'src/shared/models/ui-models/order.interface';
import { Observable } from 'rxjs';
import { environment as env } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderHttpService {

  constructor(private http: HttpClient,private authService: AuthService) { }

  getOrderDraftOfCustomer(): Observable<IOrderDraftViewModel[]> {
    let header = new HttpHeaders();
    return this.http.get<IOrderDraftViewModel[]>(`${env.BASEURL}/api/OrderCart/OrderAPI/GetDraftOrdersOfUser?userId=${this.authService.userId}`);
  }

  confirmOrderCustomerInfo(customerOrderInfoConfirmedReq: ICustomerOrderInfoConfirmedRequest) {
    return this.http.post<ICustomerOrderInfoConfirmedRequest>(`${env.BASEURL}/api/OrderCart/OrderAPI/ConfirmOrderCustomerInfo`, customerOrderInfoConfirmedReq);
  }
}
