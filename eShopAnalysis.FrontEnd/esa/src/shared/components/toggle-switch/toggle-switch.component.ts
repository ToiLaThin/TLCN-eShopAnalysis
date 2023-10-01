import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ThemeToggleService } from 'src/shared/services/theme-toggle.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'esa-toggle-switch',
  templateUrl: './toggle-switch.component.html',
  styleUrls: ['./toggle-switch.component.scss']
})
export class ToggleSwitchComponent implements OnInit {

  isDarkTheme$!: Observable<boolean>;
  constructor(private themeToggleService: ThemeToggleService) { 
    this.isDarkTheme$ = this.themeToggleService.isDarkTheme$;
  }

  ngOnInit(): void {
  }

  @Output('toggle') toggleSwitchEventEmitter = new EventEmitter<any>();

  toggle() {
    this.toggleSwitchEventEmitter.emit();
  }
}
