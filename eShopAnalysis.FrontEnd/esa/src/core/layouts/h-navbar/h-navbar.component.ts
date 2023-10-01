import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'esa-h-navbar',
  templateUrl: './h-navbar.component.html',
  styleUrls: ['./h-navbar.component.scss']
})
export class HNavbarComponent implements OnInit {

  constructor() { }

  showNotification = false;
  ngOnInit(): void {
  }

  toggleNav() {
    let ele = document.querySelector(".hnavbar-tabs") as HTMLUListElement;
    ele.classList.toggle("showNavs");
  }

  onCloseNotificationBox() {
    this.showNotification = false;
  }
}
