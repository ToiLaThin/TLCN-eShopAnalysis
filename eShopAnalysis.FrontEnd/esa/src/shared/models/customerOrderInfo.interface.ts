export interface ICustomerOrderAddress {
    //this model must match backend model if passing to backend directly through controller
    country: string;
    cityOrProvinceOrPlace: string;
    districtOrLocality: string;
    postalCode: string;
    street: string;
    fullAddressName: string;
}
export interface ICustomerOrderGeometry {
    lat: number;
    lng: number;
}

export interface ICustomerOrderInfo {
    address: ICustomerOrderAddress;
    geometry: ICustomerOrderGeometry;
    phoneNumber?: string;
}

export interface ICustomerOrderInfoConfirmedRequest {
    orderId: string;
    address: ICustomerOrderAddress;
    geometry: ICustomerOrderGeometry;
    phoneNumber: string;
}