import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubcatalogAddComponent } from './subcatalog-add.component';

describe('SubcatalogAddComponent', () => {
  let component: SubcatalogAddComponent;
  let fixture: ComponentFixture<SubcatalogAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubcatalogAddComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubcatalogAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
