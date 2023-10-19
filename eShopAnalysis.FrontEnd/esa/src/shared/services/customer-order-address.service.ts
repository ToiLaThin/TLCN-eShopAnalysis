import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { ICustomerOrderInfo } from '../models/customerOrderInfo.interface';
//should have a dependency on mapbox service to exchange geometry for address

@Injectable({
  providedIn: 'root'
})
export class CustomerOrderAddressService {
  geometryKey = 'geometry';
  addressKey = 'address';
  CustomerOrderInfoSub: BehaviorSubject<ICustomerOrderInfo | null> = new BehaviorSubject<ICustomerOrderInfo | null>(null);
  CustomerOrderInfo$ = this.CustomerOrderInfoSub.asObservable();
  isAddressDefined$: Observable<boolean> = this.CustomerOrderInfoSub.asObservable().pipe(
    map(cusInfo => {
      if (cusInfo !== null) { 
        if (cusInfo.address !== null) { return true; } 
      }
      return false;
    })
  );
  constructor() {
    this.tryGetAddressFromLocalStorage();
  }  

  //change the address and geometry subject
  private tryGetAddressFromLocalStorage(): void {
    const geometry = localStorage.getItem(this.geometryKey);
    const address = localStorage.getItem(this.addressKey);
    if (geometry !== null) {
      let geometryObj = JSON.parse(geometry);
      if (address !== null) { 
        let customerOrderInfo: ICustomerOrderInfo = { geometry: geometryObj, address: JSON.parse(address) };
        this.CustomerOrderInfoSub.next(customerOrderInfo); 
        return; 
      } else { this.exchangeGeometryForAddressThenSubNext(geometryObj); return;}
    }
    this.CustomerOrderInfoSub.next(null);
  }

  private exchangeGeometryForAddressThenSubNext(geometry: Object): void {
    //TODO: exchange geometry for address with geocoding mapbox api
    let addressExchanged = {
      country: "Viet Nam",
      cityOrProvinceOrPlace: "Ho Chi Minh City",
      districtOrLocality: "Thu Duc",
      postalCode: "700000",
      street: "123 Nguyen Van Linh",
      fullAddressName: "123 Nguyen Van Linh, Thu Duc, Ho Chi Minh City, Viet Nam",
    }
    localStorage.setItem(this.addressKey, JSON.stringify(addressExchanged));
    this.CustomerOrderInfoSub.next({ geometry: geometry, address: addressExchanged } as ICustomerOrderInfo);
  }

  //user agree to use navigator.geolocation.getCurrentPosition
  setGeometry(geometry: Object): void {
    localStorage.setItem(this.geometryKey, JSON.stringify(geometry));
    this.tryGetAddressFromLocalStorage();
  }

  removeGeometry(): void {
    localStorage.removeItem(this.geometryKey);
    localStorage.removeItem(this.addressKey);
    this.tryGetAddressFromLocalStorage();
  }
}
