import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ICustomerOrderInfoConfirmedRequest } from 'src/shared/models/customerOrderInfo.interface';
import { AuthService } from '../auth.service';
import { IOrderDraftViewModel, IOrderViewModel, OrderStatus } from 'src/shared/models/ui-models/order.interface';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment as env } from 'src/environments/environment';
import { IPaymentRequest, IPaymentResponse } from 'src/shared/models/payment.interface';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class OrderHttpService {

  trackingOrderKey = 'trackingOrderId';
  trackingOrderSub: BehaviorSubject<IOrderViewModel | null> = new BehaviorSubject<IOrderViewModel | null>(null);
  trackingOrder$ = this.trackingOrderSub.asObservable();
  get trackingOrderGetter() { return this.trackingOrderSub.value; }
  
  constructor(private http: HttpClient,
              private authService: AuthService, 
              private router: Router) {
    let orderJsonStr = localStorage.getItem(this.trackingOrderKey);
    if (orderJsonStr !== null) {
      let order = JSON.parse(orderJsonStr);
      this.trackingOrderSub.next(order);
    }
   }

  getOrderDraftOfCustomer(): Observable<IOrderDraftViewModel[]> {
    let header = new HttpHeaders();
    return this.http.get<IOrderDraftViewModel[]>(`${env.BASEURL}/api/OrderCart/OrderAPI/GetDraftOrdersOfUser?userId=${this.authService.userId}`);
  }

  confirmOrderCustomerInfo(customerOrderInfoConfirmedReq: ICustomerOrderInfoConfirmedRequest) {
    return this.http.post<ICustomerOrderInfoConfirmedRequest>(`${env.BASEURL}/api/OrderCart/OrderAPI/ConfirmOrderCustomerInfo`, customerOrderInfoConfirmedReq);
  }

  pickPaymentMethodCOD() {
    let orderId = this.trackingOrderSub.value?.orderId;
    if (orderId === undefined || orderId === null) { alert("There is an exception"); return; }
    return this.http.put<IOrderViewModel>(`${env.BASEURL}/api/OrderCart/OrderAPI/PickPaymentMethodCOD?orderId=${orderId}`, {});
  }

  pickPaymentMethodEWallet() {
    let order = this.trackingOrderSub.value;
    if (order === null) { alert("There is an exception"); return; }
    let paymentRequest: IPaymentRequest = {
      userId: this.authService.userId,
      orderId: order.orderId,
      subTotal: order.subTotal,
      totalDiscount: order.totalDiscount
    }
    return this.http.post<IPaymentResponse>(`${env.BASEURL}/api/Payment/MomoAPI/MakePayment`, paymentRequest);
  }

  //TODO vì việc thanh toán thành công được xử lý ở webhook và webhook trả kq về cho stripe server => 
  //phải thông báo cho user thanh toán thàng công qua signalR notification hub để stopTracking order
  //lúc này sẽ có 1 trang để list các order đã xong và chờ được duyệt
  pickPaymentMethodCreditCard() {
    let order = this.trackingOrderSub.value;
    if (order === null) { alert("There is an exception"); return; }
    let paymentRequest: IPaymentRequest = {
      userId: this.authService.userId,
      orderId: order.orderId,
      subTotal: order.subTotal,
      totalDiscount: order.totalDiscount
    }
    return this.http.post<IPaymentResponse>(`${env.BASEURL}/api/Payment/StripeAPI/MakePayment`, paymentRequest);
  }

  beginTrackingOrder(orderDraft: IOrderDraftViewModel) {
    let order = Object.assign(
      {
        orderStatus: OrderStatus.createdDraft
      }, orderDraft
    );
    this.trackingOrderSub.next(order);
    localStorage.setItem(this.trackingOrderKey, JSON.stringify(order));
  }

  markCurrentTrackingOrderAsConfirmedInfo() {
    let order = this.trackingOrderSub.value;
    if (order === null) { return; }
    order.orderStatus = OrderStatus.customerInfoConfirmed;
    this.trackingOrderSub.next(order);
    localStorage.setItem(this.trackingOrderKey, JSON.stringify(order));
  }


  stopTrackingOrder() { 
    this.trackingOrderSub.next(null);
    localStorage.removeItem(this.trackingOrderKey); 
  }

  //TODO use router to navigate to the the page that you left in the ordering process
  //TODO store the url of the page in the ordering process as a key in the local storage or use any other method(here i store the status of tracking order in local storage)
  //to determine which page to navigate to
  goBackToWhereYouLeftOf() {
    let order = this.trackingOrderSub.value;
    if (order === null) { alert("No order tracking. This is an exception"); return; }
    if (order.orderStatus === OrderStatus.createdDraft) { 
      this.router.navigate(['/shopping/ordering-info-confirm', order.orderId], { replaceUrl: true }); 
    } else if (order.orderStatus === OrderStatus.customerInfoConfirmed) {
      //if payment method is COD, the status is still customerInfoConfirmed, so when the response of pickPaymentMethodCOD comeback,
      //we must stop tracking the order to prevent the user from going back to this page
      this.router.navigate(['/shopping/pick-payment-method'], { replaceUrl: true }); 
    }
  }

}
