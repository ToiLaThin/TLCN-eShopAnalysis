import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, map } from 'rxjs';
import { ICustomerOrderInfoConfirmedRequest } from 'src/shared/models/customerOrderInfo.interface';
import { CustomerOrderAddressService } from 'src/shared/services/customer-order-address.service';
import { OrderHttpService } from 'src/shared/services/http/order-http.service';

@Component({
  selector: 'esa-ordering-info-confirm',
  templateUrl: './ordering-info-confirm.component.html',
  styleUrls: ['./ordering-info-confirm.component.scss']
})
export class OrderingInfoConfirmComponent implements OnInit {

  orderId!: string;
  inputtedPhoneNumber!: string;
  customerInfoAddressConfirmedReq!: ICustomerOrderInfoConfirmedRequest;
  isAddressDefined$!: Observable<boolean>;
  inputedAddress$!: Observable<string>;
  constructor(
    private addressService: CustomerOrderAddressService,
    private orderService: OrderHttpService, 
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => { this.orderId = params['orderId']; })
                     .unsubscribe();
    this.isAddressDefined$ = this.addressService.isAddressDefined$;
    this.inputedAddress$ = this.addressService.CustomerOrderInfo$.pipe(
      map(cus => {
        if (cus !== null) {
          return cus.address.fullAddressName;
        }
        else { return ''; }
      })
    );
  }

  useGeoLocationApi() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          this.addressService.setGeometry({ lat: position.coords.latitude, lng: position.coords.longitude });
        }
      );
    } else {
      alert("Geolocation is not supported by this browser.");
    }
  }

  removeMyAddress() {
    this.addressService.removeGeometry();
  }

  confirmCurrentInfo() {
    if (this.inputtedPhoneNumber === undefined || this.inputtedPhoneNumber === null || this.inputtedPhoneNumber === '') {
      alert('Please input your phone number');
      return;
    }

    this.addressService.CustomerOrderInfo$.subscribe(cusInfo => {
      if (cusInfo !== null) {
        this.customerInfoAddressConfirmedReq = {
          orderId: this.orderId,
          address: cusInfo.address,
          geometry: cusInfo.geometry,
          phoneNumber: this.inputtedPhoneNumber
        };
      } else {
        alert('Please input your address');
        return;
      }
    }).unsubscribe();

    console.log("Request to be sent to server", this.customerInfoAddressConfirmedReq);
    this.orderService.confirmOrderCustomerInfo(this.customerInfoAddressConfirmedReq).subscribe(
      (resultFromServer) => {
        console.log("Info confirm result", resultFromServer);
        this.orderService.markCurrentTrackingOrderAsConfirmedInfo();
        this.router.navigate(['/shopping/pick-payment-method'], { replaceUrl: true});
      }
    )
  }
}
