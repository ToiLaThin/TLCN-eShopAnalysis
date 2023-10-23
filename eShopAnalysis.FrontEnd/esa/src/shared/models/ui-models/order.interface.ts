export enum OrderStatus {
    createdDraft = 0,
    customerInfoConfirmed = 1,
    checkouted = 2,
    stockConfirmed = 3,
    refunded = 4,
    cancelled = 5,
    completed = 6
}

export interface IOrderDraftViewModel {
    orderId: string;
    subTotal: number;
    totalDiscount: number;
}

export interface IOrderViewModel {
    orderId: string;
    orderStatus: OrderStatus;
    subTotal: number;
    totalDiscount: number;
}