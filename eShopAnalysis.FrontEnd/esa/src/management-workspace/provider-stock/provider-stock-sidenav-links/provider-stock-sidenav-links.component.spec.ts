import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProviderStockSidenavLinksComponent } from './provider-stock-sidenav-links.component';

describe('ProviderStockSidenavLinksComponent', () => {
  let component: ProviderStockSidenavLinksComponent;
  let fixture: ComponentFixture<ProviderStockSidenavLinksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProviderStockSidenavLinksComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProviderStockSidenavLinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
