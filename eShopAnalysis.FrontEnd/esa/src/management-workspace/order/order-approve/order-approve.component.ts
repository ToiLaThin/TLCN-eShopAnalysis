import { Component, OnInit } from '@angular/core';
import { OrderApproveService } from './order-approve.service';
import { Observable, map, tap } from 'rxjs';
import { IItemStock, IOrderItems } from './order-approve.model';
import { OrderStatus } from 'src/shared/types/orderStatus.enum';
import { PaymentMethod } from 'src/shared/types/paymentMethod.enum';
@Component({
  selector: 'esa-order-approve',
  templateUrl: './order-approve.component.html',
  styleUrls: ['./order-approve.component.scss']
})
export class OrderApproveComponent implements OnInit {

  itemsStockLookUp$!: Observable<IItemStock[]>;
  ordersToApprove$!: Observable<IOrderItems[]>;
  get PaymentMethod() { return PaymentMethod; }
  get OrderStatus() { return OrderStatus; }

  constructor(private orderApproveService: OrderApproveService) { 
    this.orderApproveService.reset();
  }

  
  ngOnInit(): void {
    this.itemsStockLookUp$ = this.orderApproveService.itemsStockLookUp$.pipe(
      tap((m) => console.log(m)),
      map((m) => {
        let result: IItemStock[] = [];
        m.forEach((v, k) => { result.push({ productModelId: k, quantity: v }); })
        return result;
      })
    );
    this.ordersToApprove$ = this.orderApproveService.ordersToApprove$;
  }

  approveOrder(orderId: string) {
    this.orderApproveService.approveOrder(orderId);
  }

  reset() {
    this.orderApproveService.reset();
  }

  confirm() {
    this.orderApproveService.confirmApprovingOrders();
  }
}
