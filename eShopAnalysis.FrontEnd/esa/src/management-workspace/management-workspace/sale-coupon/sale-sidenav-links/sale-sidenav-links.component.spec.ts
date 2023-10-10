import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleSidenavLinksComponent } from './sale-sidenav-links.component';

describe('SaleSidenavLinksComponent', () => {
  let component: SaleSidenavLinksComponent;
  let fixture: ComponentFixture<SaleSidenavLinksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaleSidenavLinksComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SaleSidenavLinksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
