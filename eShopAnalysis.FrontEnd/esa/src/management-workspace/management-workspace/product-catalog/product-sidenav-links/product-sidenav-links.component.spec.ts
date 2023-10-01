import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductSidenavLinksComponent } from './product-sidenav-links.component';

describe('ProductSidenavLinksComponent', () => {
  let component: ProductSidenavLinksComponent;
  let fixture: ComponentFixture<ProductSidenavLinksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProductSidenavLinksComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductSidenavLinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
