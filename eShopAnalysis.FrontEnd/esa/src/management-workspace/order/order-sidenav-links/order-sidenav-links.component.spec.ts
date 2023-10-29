import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderSidenavLinksComponent } from './order-sidenav-links.component';

describe('OrderSidenavLinksComponent', () => {
  let component: OrderSidenavLinksComponent;
  let fixture: ComponentFixture<OrderSidenavLinksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderSidenavLinksComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderSidenavLinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
