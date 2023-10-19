import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderingInfoConfirmComponent } from './ordering-info-confirm.component';

describe('OrderingInfoConfirmComponent', () => {
  let component: OrderingInfoConfirmComponent;
  let fixture: ComponentFixture<OrderingInfoConfirmComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderingInfoConfirmComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderingInfoConfirmComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
