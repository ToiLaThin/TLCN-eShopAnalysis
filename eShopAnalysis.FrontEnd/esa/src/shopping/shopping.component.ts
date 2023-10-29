import { Component, OnInit } from '@angular/core';
import { SignalrService } from 'src/shared/services/signalr.service';

@Component({
  selector: 'esa-shopping',
  templateUrl: './shopping.component.html',
  styleUrls: ['./shopping.component.scss']
})
export class ShoppingComponent implements OnInit {

  constructor(private signalrService: SignalrService) {
  }
  
  ngOnInit(): void {
    this.signalrService.initConnection();
  }

}
