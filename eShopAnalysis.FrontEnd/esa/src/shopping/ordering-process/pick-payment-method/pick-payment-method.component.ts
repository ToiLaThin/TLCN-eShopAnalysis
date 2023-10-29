import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PaymentMethod } from 'src/shared/types/paymentMethod.enum';
import { OrderHttpService } from 'src/shared/services/http/order-http.service';

@Component({
  selector: 'esa-pick-payment-method',
  templateUrl: './pick-payment-method.component.html',
  styleUrls: ['./pick-payment-method.component.scss']
})
export class PickPaymentMethodComponent implements OnInit {

  constructor(private orderService: OrderHttpService,
              private router: Router) { }

  ngOnInit(): void {
    alert('Please choose a payment method');
  }

  paymentMethodKeyArr = Object.keys(PaymentMethod).map(p => parseInt(p)).filter(x => !isNaN(x));
  paymentMethodKeyValueArr = this.paymentMethodKeyArr.map((key) => {
    return {
      key,
      value: PaymentMethod[key]
    }
  });

  chooseThisPaymentMethod() {
    let chosenPaymentMethodInput = document.querySelector('input[name="payment-methods"]:checked') as HTMLInputElement;
    if (chosenPaymentMethodInput === null) {
      alert('Please choose a payment method');
      return;
    }
    let currentTrackingOrder = this.orderService.trackingOrderGetter;
    if (currentTrackingOrder === null) { 
      alert('There is an error. Please try again later'); 
      return;
    }

    let selectedPaymentMethod = this.paymentMethodKeyValueArr.find(p => p.key === parseInt(chosenPaymentMethodInput.value));
    switch(selectedPaymentMethod?.key) {
      case PaymentMethod.COD:
        //TODO: add an route guard before navigating to this component to make sure that the orderId is not undefined
        this.orderService.pickPaymentMethodCOD()?.subscribe(
          () => {
            this.orderService.stopTrackingOrder();
            this.router.navigate(['shopping/notify-customer-observe-order']);
          }
        );
        break;
      case PaymentMethod.Momo:
        this.orderService.pickPaymentMethodEWallet()?.subscribe((paymentResponse) => {
          console.log('Momo response came backed from server');
          if (paymentResponse === null) {
            alert('There is an error. Please try again later');
            return;
          } else if (paymentResponse.payUrl === null || paymentResponse.payUrl === "") {
            alert('There is an error. Payment failed. Please try again later');
            return;
          }
          window.location.href = paymentResponse.payUrl; //go to stripe host checkotu page
        }
      );
      break;
      case PaymentMethod.CreditCard:
        this.orderService.pickPaymentMethodCreditCard()?.subscribe((paymentResponse) => {
            console.log('Credit card chosen come backed from server');
            if (paymentResponse === null) {
              alert('There is an error. Please try again later');
              return;
            } else if (paymentResponse.payUrl === null || paymentResponse.payUrl === "") {
              alert('There is an error. Payment failed. Please try again later');
              return;
            }
            window.location.href = paymentResponse.payUrl; //go to stripe host checkotu page
          }
        );
        break;
    }
  }

}
