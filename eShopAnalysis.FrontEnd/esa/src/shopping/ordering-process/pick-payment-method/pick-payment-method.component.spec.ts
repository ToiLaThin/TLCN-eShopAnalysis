import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PickPaymentMethodComponent } from './pick-payment-method.component';

describe('PickPaymentMethodComponent', () => {
  let component: PickPaymentMethodComponent;
  let fixture: ComponentFixture<PickPaymentMethodComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PickPaymentMethodComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PickPaymentMethodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
