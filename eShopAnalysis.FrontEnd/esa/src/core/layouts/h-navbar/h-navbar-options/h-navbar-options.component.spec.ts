import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HNavbarOptionsComponent } from './h-navbar-options.component';

describe('HNavbarOptionsComponent', () => {
  let component: HNavbarOptionsComponent;
  let fixture: ComponentFixture<HNavbarOptionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HNavbarOptionsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HNavbarOptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
