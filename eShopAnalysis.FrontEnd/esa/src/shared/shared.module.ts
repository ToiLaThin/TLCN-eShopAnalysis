import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DropdownMenuComponent } from './components/dropdown-menu/dropdown-menu.component';
import { ToggleSwitchComponent } from './components/toggle-switch/toggle-switch.component';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoaderSpinnerComponent } from './components/loader-spinner/loader-spinner.component';



@NgModule({
  declarations: [
    DropdownMenuComponent,
    ToggleSwitchComponent,
    LoaderSpinnerComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ],
  exports: [
    DropdownMenuComponent,
    ToggleSwitchComponent,
    LoaderSpinnerComponent,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class SharedModule { }
