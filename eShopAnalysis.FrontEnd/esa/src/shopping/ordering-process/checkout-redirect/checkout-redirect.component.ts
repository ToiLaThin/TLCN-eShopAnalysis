import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SignalrService } from 'src/shared/services/signalr.service';

@Component({
  selector: 'esa-checkout-redirect',
  template: '',
  styles: []
})
export class CheckoutRedirectComponent implements OnInit {

  constructor(private router: Router,private signalrService: SignalrService) { }

  ngOnInit(): void {
    this.signalrService.initConnection();
    this.router.navigate(['/shopping/notify-customer-observe-order'], { replaceUrl: true });
  }

}
