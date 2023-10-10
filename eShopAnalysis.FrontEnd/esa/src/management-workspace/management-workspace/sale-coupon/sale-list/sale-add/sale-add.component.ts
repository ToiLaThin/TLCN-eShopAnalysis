import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DiscountType } from 'src/shared/models/saleItem.interface';
import { SaleHttpService } from 'src/shared/services/http/sale-http.service';

@Component({
  selector: 'esa-sale-add',
  templateUrl: './sale-add.component.html',
  styleUrls: ['./sale-add.component.scss']
})
export class SaleAddComponent implements OnInit {

  constructor(private fb: FormBuilder, private saleHttpService: SaleHttpService) { }
  
  ngOnInit(): void {
    this.modelSaleFrmGrp = this.fb.group({
      productId: [this.productId],
      productModelId: [this.productModelId],
      businessKey: [this.businessKey],
      discountType: [DiscountType.ByValue],
      discountValue: [0],    
    })
  }
  
  @Input() title: string = "";
  @Input('productId') productId: string | undefined  = "";
  @Input('productModelId') productModelId: string | undefined = "";
  @Input('businessKey') businessKey: string | undefined = "";
  @Output('onCloseBtnClick') btnCloseClickEvent: EventEmitter<any> = new EventEmitter<any>();
  modelSaleFrmGrp!: FormGroup;
  
  BtnCloseClickHandler() {
    this.btnCloseClickEvent.emit();
  }


  discountTypeKeyArr = Object.keys(DiscountType)
                             .map(x => parseInt(x))
                             .filter(x => !isNaN(x));
  discountTypeKeyValueArr = this.discountTypeKeyArr.map(key => {
    return {
      key: key,
      value: DiscountType[key]
    }
  });

  addSaleToModel() {
    console.log(this.modelSaleFrmGrp.value);
    
    this.saleHttpService.AddSaleToModel(this.modelSaleFrmGrp.value);
  }
}
