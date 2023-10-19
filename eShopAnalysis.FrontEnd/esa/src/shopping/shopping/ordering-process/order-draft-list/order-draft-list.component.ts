import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IOrderDraftViewModel } from 'src/shared/models/ui-models/order.interface';
import { OrderHttpService } from 'src/shared/services/http/order-http.service';

@Component({
  selector: 'esa-order-draft-list',
  templateUrl: './order-draft-list.component.html',
  styleUrls: ['./order-draft-list.component.scss']
})
export class OrderDraftListComponent implements OnInit {

  orderDraft$!: Observable<IOrderDraftViewModel[]>;
  constructor(private orderService: OrderHttpService,private router: Router) { }

  ngOnInit(): void {
    this.orderDraft$ = this.orderService.getOrderDraftOfCustomer();
    this.orderService.getOrderDraftOfCustomer().subscribe(res => console.log(res));
  }

  confirmOderingInfo(orderId: string) {
    this.router.navigate(['/shopping/ordering-info-confirm', orderId], { replaceUrl: true});
  }
}
