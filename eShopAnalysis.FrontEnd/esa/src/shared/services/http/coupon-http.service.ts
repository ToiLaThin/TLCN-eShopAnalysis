import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment as env } from 'src/environments/environment';
import { ICoupon } from 'src/shared/models/coupon.interface';
@Injectable({
  providedIn: 'root'
})
export class CouponHttpService {

  constructor(private http: HttpClient) { 
    this.getAllCoupons().subscribe((coupons) => {
      this.allCouponsSub.next(coupons);
    });
  }

  allCouponsSub: BehaviorSubject<ICoupon[]> = new BehaviorSubject<ICoupon[]>([]);
  allCoupons$ = this.allCouponsSub.asObservable();

  public getAllCoupons() {
    return this.http.get<ICoupon[]>(`${env.BASEURL}/api/CouponSaleItem/CouponAPI/GetAllCoupons`);
  }

  public addCoupon(coupon: ICoupon) {
    return this.http.post<ICoupon>(`${env.BASEURL}/api/CouponSaleItem/CouponAPI/AddCoupon`, coupon);
  }
  
  
}
