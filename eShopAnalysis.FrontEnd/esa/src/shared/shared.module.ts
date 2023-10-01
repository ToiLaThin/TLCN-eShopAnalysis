import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DropdownMenuComponent } from './components/dropdown-menu/dropdown-menu.component';
import { ToggleSwitchComponent } from './components/toggle-switch/toggle-switch.component';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoaderSpinnerComponent } from './components/loader-spinner/loader-spinner.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatTabsModule} from '@angular/material/tabs';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatCardModule} from '@angular/material/card';
import {MatButtonModule} from '@angular/material/button';
import {MatRadioModule} from '@angular/material/radio';
import {MatSelectModule} from '@angular/material/select';
import {MatTreeModule} from '@angular/material/tree';
import {MatTableModule} from '@angular/material/table';
import {MatStepperModule} from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
// import { MatFormFieldModule } from "@angular/material/form-field";
import {MatCheckboxModule} from '@angular/material/checkbox';
import { GgAnalyticsService } from './services/gg-analytics.service';
import { MatListModule } from '@angular/material/list';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatMenuModule } from '@angular/material/menu';
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
    ReactiveFormsModule,
    MatToolbarModule,
    MatIconModule,
    MatSidenavModule,
    MatTreeModule,
    MatTabsModule,
    MatTooltipModule,
    MatCardModule,
    MatButtonModule,
    MatRadioModule,
    MatSelectModule,//can be import in the component level
    MatTableModule,
    MatStepperModule,
    MatInputModule,
    MatCheckboxModule,
    MatListModule,
    MatGridListModule,
    MatMenuModule
  ],
  exports: [
    DropdownMenuComponent,
    ToggleSwitchComponent,
    LoaderSpinnerComponent,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatSidenavModule,
    MatIconModule,
    MatToolbarModule,
    MatIconModule,
    MatSidenavModule,
    MatTreeModule,
    MatTabsModule,
    MatTooltipModule,
    MatCardModule,
    MatButtonModule,
    MatRadioModule,
    MatSelectModule,
    MatTableModule,
    MatStepperModule,
    MatInputModule,
    MatCheckboxModule,
    MatListModule,
    MatGridListModule,
    MatMenuModule
  ],
  providers: [
    GgAnalyticsService
  ]
})
export class SharedModule { }
