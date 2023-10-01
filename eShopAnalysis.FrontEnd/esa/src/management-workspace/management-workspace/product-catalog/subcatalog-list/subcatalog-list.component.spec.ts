import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubcatalogListComponent } from './subcatalog-list.component';

describe('SubcatalogListComponent', () => {
  let component: SubcatalogListComponent;
  let fixture: ComponentFixture<SubcatalogListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SubcatalogListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubcatalogListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
