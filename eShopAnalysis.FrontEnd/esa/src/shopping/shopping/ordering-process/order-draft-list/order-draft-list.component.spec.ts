import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderDraftListComponent } from './order-draft-list.component';

describe('OrderDraftListComponent', () => {
  let component: OrderDraftListComponent;
  let fixture: ComponentFixture<OrderDraftListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OrderDraftListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrderDraftListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
