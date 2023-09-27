import { Component, OnInit } from '@angular/core';
import { ShowDropDownService } from 'src/shared/services/show-drop-down.service';

@Component({
  selector: 'esa-dropdown-menu',
  templateUrl: './dropdown-menu.component.html',
  styleUrls: ['./dropdown-menu.component.scss']
})
export class DropdownMenuComponent implements OnInit {

  constructor(private showDropDownService: ShowDropDownService) { }

  ngOnInit(): void {
  }

  toggleHNavBarDropDown() {
    this.showDropDownService.toggleHNavBarDropDown();
  }

}
