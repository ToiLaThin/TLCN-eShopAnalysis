export interface IPaymentRequest {
    userId: string;
    orderId: string;
    cardId?: string;
    subTotal: number;
    totalDiscount: number;
}

export interface IPaymentResponse {
    payUrl: string;
}