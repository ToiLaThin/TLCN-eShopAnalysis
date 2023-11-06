import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment as env } from 'src/environments/environment';
import { IItemStock, IOrderApprovedAggregate, IOrderItems, IOrderItemsAndStockAggregate } from './order-approve.model';
import { HttpClient } from '@angular/common/http';
@Injectable({
  providedIn: 'root'
})
export class OrderApproveService {

  itemsStockLookUpSub: BehaviorSubject<Map<string, number>> = new BehaviorSubject<Map<string, number>>(new Map<string, number>());
  itemsStockLookUp$: Observable<Map<string, number>> = this.itemsStockLookUpSub.asObservable();
  itemsStockLookUpGetter(): Map<string, number> { return this.itemsStockLookUpSub.getValue(); }

  ordersToApproveSub: BehaviorSubject<IOrderItems[]> = new BehaviorSubject<IOrderItems[]>([]);
  ordersToApprove$: Observable<IOrderItems[]> = this.ordersToApproveSub.asObservable();
  ordersToApproveGetter(): IOrderItems[] { return this.ordersToApproveSub.getValue(); }

  ordersApprovedSub: BehaviorSubject<IOrderApprovedAggregate[]> = new BehaviorSubject<IOrderApprovedAggregate[]>([]);
  ordersApprovedGetter(): IOrderApprovedAggregate[] { return this.ordersApprovedSub.getValue(); }

  constructor(private http: HttpClient) {
    this.reset();
  }

  public reset() {
    this.getBatchOrderApprove().subscribe(res => {
      //must parse it mannulaly      
      if (res != null) {
        res.itemsStock = new Map<string, number>(Object.entries(res.itemsStock));
        this.itemsStockLookUpSub.next(res.itemsStock);
        this.ordersToApproveSub.next(res.orderItems);
        this.ordersApprovedSub.next([]); //without this the next time we approve order, the previous approved order(changed status) will be there which is false
        return;
      }      
      this.itemsStockLookUpSub.next(new Map<string, number>());
      this.ordersToApproveSub.next([]);
      this.ordersApprovedSub.next([]); 
    });
  }

  public getBatchOrderApprove() {
    return this.http.get<IOrderItemsAndStockAggregate>(`${env.BASEURL}/api/Aggregate/ReadAggregator/GetOrderToApproveWithStock`);
  }

  public approveOrder(orderId: string) {
    let order: IOrderItems | undefined = this.ordersToApproveGetter().find(x => x.orderId == orderId);
    if (order) { //if order is found
      let orderItemsQty: IItemStock[] = order.orderItemsQty;
      let itemsStockLookUpTemp = this.itemsStockLookUpGetter();
      let allItemsIsValid = true;
      orderItemsQty.forEach(x => { 
        let stockCurrentQuantity = this.itemsStockLookUpGetter().get(x.productModelId);
        if (stockCurrentQuantity) {
          itemsStockLookUpTemp.set(x.productModelId, stockCurrentQuantity - x.quantity);
        } else { allItemsIsValid = false; }
      });
      if (allItemsIsValid) {
        this.itemsStockLookUpSub.next(itemsStockLookUpTemp);
        this.ordersToApproveSub.next(this.ordersToApproveGetter().filter(x => x.orderId != orderId));
        this.ordersApprovedSub.next([...this.ordersApprovedGetter(), 
          { orderId: orderId, orderItemsStockToChange: orderItemsQty.map(
            (x) => { 
              return { productModelId: x.productModelId, quantityToDecrease: x.quantity 
            }; 
          })}
        ]);
        return;
      }
    }
    alert("Order is not valid");
  }

  public confirmApprovingOrders() {
    let ordersApproved = this.ordersApprovedGetter();
    if (ordersApproved.length > 0) {
      this.http.post(`${env.BASEURL}/api/Aggregate/WriteAggregator/ApproveOrdersAndModifyStocks`, ordersApproved).subscribe(res => {
        this.reset();
        alert("Order approved successfully");
      });
    }    
  }
}
