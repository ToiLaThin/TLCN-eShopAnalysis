import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ICustomerOrderInfoConfirmedRequest } from 'src/shared/models/customerOrderInfo.interface';
import { AuthService } from '../auth.service';
import { IOrderDraftViewModel, IOrderViewModel } from 'src/shared/models/ui-models/order.interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment as env } from 'src/environments/environment';
import { PaymentMethod } from 'src/shared/models/ui-models/paymentMethod.enum';

@Injectable({
  providedIn: 'root'
})
export class OrderHttpService {

  trackingOrderKey = 'trackingOrderId';
  trackingOrderId!: string | null;
  constructor(private http: HttpClient,private authService: AuthService) {
    this.trackingOrderId = localStorage.getItem(this.trackingOrderKey);
   }

  getOrderDraftOfCustomer(): Observable<IOrderDraftViewModel[]> {
    let header = new HttpHeaders();
    return this.http.get<IOrderDraftViewModel[]>(`${env.BASEURL}/api/OrderCart/OrderAPI/GetDraftOrdersOfUser?userId=${this.authService.userId}`);
  }

  confirmOrderCustomerInfo(customerOrderInfoConfirmedReq: ICustomerOrderInfoConfirmedRequest) {
    return this.http.post<ICustomerOrderInfoConfirmedRequest>(`${env.BASEURL}/api/OrderCart/OrderAPI/ConfirmOrderCustomerInfo`, customerOrderInfoConfirmedReq);
  }

  pickPaymentMethodCOD(orderId: string) {
    return this.http.put<IOrderViewModel>(`${env.BASEURL}/api/OrderCart/OrderAPI/PickPaymentMethodCOD?orderId=${orderId}`, {});
  }

  trackOrder(orderId: string) { 
    this.trackingOrderId = orderId; 
    localStorage.setItem(this.trackingOrderKey, orderId);
  }

  stopTrackingOrder() { 
    this.trackingOrderId = null; 
    localStorage.removeItem(this.trackingOrderKey); 
  }

  //TODO use router to navigate to the the page that you left in the ordering process
  //TODO store the url of the page in the ordering process as a key in the local storage
  goBackToWhereYouLeftOf() {}

}
