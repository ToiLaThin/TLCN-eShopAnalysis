import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProviderStockComponent } from './provider-stock.component';

describe('ProviderStockComponent', () => {
  let component: ProviderStockComponent;
  let fixture: ComponentFixture<ProviderStockComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProviderStockComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProviderStockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
