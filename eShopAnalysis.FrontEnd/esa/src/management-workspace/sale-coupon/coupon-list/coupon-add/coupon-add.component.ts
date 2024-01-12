import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ICoupon } from 'src/shared/models/coupon.interface';
import { DiscountType, Status } from 'src/shared/models/saleItem.interface';
import { CouponHttpService } from 'src/shared/services/http/coupon-http.service';

@Component({
  selector: 'esa-coupon-add',
  templateUrl: './coupon-add.component.html',
  styleUrls: ['./coupon-add.component.scss']
})
export class CouponAddComponent implements OnInit {

  constructor(private fb: FormBuilder, 
              private couponService: CouponHttpService) { }

  ngOnInit(): void {
    this.modelCouponFrmGrp = this.fb.group({
      couponCode: [''],
      discountType: [DiscountType.ByPercent],
      discountValue: [2],
      minOrderValueToApply: [0],
      dateAdded: [Date.now()],
      dateEnded: [Date.now()],
      couponStatus: [Status.Active],
      rewardPointRequire: [0]      
    })
  }

  modelCouponFrmGrp!: FormGroup;
  discountTypeKeyArr = Object.keys(DiscountType)
                             .map(x => parseInt(x))
                             .filter(x => !isNaN(x) && x !== DiscountType.NoDiscount);
  discountTypeKeyValueArr = this.discountTypeKeyArr.map(key => {
    return {
      key: key,
      value: DiscountType[key]
    }
  });

  addNewCoupon() {
    console.log(this.modelCouponFrmGrp.value);
    let newCoupon: ICoupon = {
      couponCode: this.modelCouponFrmGrp.value.couponCode,
      discountType: this.modelCouponFrmGrp.value.discountType,
      discountValue: this.modelCouponFrmGrp.value.discountValue,
      minOrderValueToApply: this.modelCouponFrmGrp.value.minOrderValueToApply,
      dateAdded: new Date(this.modelCouponFrmGrp.value.dateAdded),
      dateEnded: new Date(this.modelCouponFrmGrp.value.dateEnded),
      couponStatus: Status.Active,
      rewardPointRequire: this.modelCouponFrmGrp.value.rewardPointRequire

    }
    console.log(newCoupon);
    console.log(typeof(newCoupon.dateAdded));
    console.log(typeof(newCoupon.dateEnded));
    
    
    this.couponService.addCoupon(newCoupon).subscribe((coupon) => {
      console.log(coupon);
    });
  }

}
