import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotifyCustomerObserveOrderComponent } from './notify-customer-observe-order.component';

describe('NotifyCustomerObserveOrderComponent', () => {
  let component: NotifyCustomerObserveOrderComponent;
  let fixture: ComponentFixture<NotifyCustomerObserveOrderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NotifyCustomerObserveOrderComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NotifyCustomerObserveOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
