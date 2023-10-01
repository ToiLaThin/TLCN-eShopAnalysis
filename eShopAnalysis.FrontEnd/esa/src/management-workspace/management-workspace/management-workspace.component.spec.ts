import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagementWorkspaceComponent } from './management-workspace.component';

describe('ManagementWorkspaceComponent', () => {
  let component: ManagementWorkspaceComponent;
  let fixture: ComponentFixture<ManagementWorkspaceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManagementWorkspaceComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManagementWorkspaceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
