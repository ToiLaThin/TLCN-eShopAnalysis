import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ICoupon } from 'src/shared/models/coupon.interface';
import { CouponHttpService } from 'src/shared/services/http/coupon-http.service';

@Component({
  selector: 'esa-coupon-list',
  templateUrl: './coupon-list.component.html',
  styleUrls: ['./coupon-list.component.scss']
})
export class CouponListComponent implements OnInit {

  constructor(private couponService: CouponHttpService,
              private router: Router
    ) { }

  allCoupons$!: Observable<ICoupon[]>;
  ngOnInit(): void {
    this.allCoupons$ = this.couponService.allCoupons$;
  }

  navigateToAddCoupon() {
    this.router.navigate(['/management/sale-coupon/coupon-add'], { replaceUrl: true});
  }


}
