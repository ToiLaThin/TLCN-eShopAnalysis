import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment as env } from 'src/environments/environment';
import { ICoupon } from 'src/shared/models/coupon.interface';
import { AuthService } from '../auth.service';
@Injectable({
  providedIn: 'root'
})
export class CouponHttpService {

  constructor(private http: HttpClient, private authService: AuthService) { 
    this.getAllCoupons().subscribe((coupons) => {
      this.allCouponsSub.next(coupons);
    });
    this.getAllActiveCouponsNotUsedByUser().subscribe((coupons) => {
      this.allActiveCouponsNotUsedByUserSub.next(coupons);
      console.log(coupons);
      
    });
  }

  allCouponsSub: BehaviorSubject<ICoupon[]> = new BehaviorSubject<ICoupon[]>([]);
  allActiveCouponsNotUsedByUserSub: BehaviorSubject<ICoupon[]> = new BehaviorSubject<ICoupon[]>([]);
  allCoupons$ = this.allCouponsSub.asObservable();
  allActiveCouponsNotUsedByUser$ = this.allActiveCouponsNotUsedByUserSub.asObservable();
  public getAllCoupons() {
    return this.http.get<ICoupon[]>(`${env.BASEURL}/api/CouponSaleItem/CouponAPI/GetAllCoupons`);
  }


  public addCoupon(coupon: ICoupon) {
    return this.http.post<ICoupon>(`${env.BASEURL}/api/CouponSaleItem/CouponAPI/AddCoupon`, coupon);
  }  

  public getAllActiveCouponsNotUsedByUser() {
    let userId: string = this.authService.userId;
    return this.http.get<ICoupon[]>(`${env.BASEURL}/api/CouponSaleItem/CouponAPI/GetAllActiveCouponsNotUsedByUser?userId=${userId}`);
  }
  
  
}
