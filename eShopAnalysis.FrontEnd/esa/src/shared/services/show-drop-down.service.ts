import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShowDropDownService {

  showHNavBarDropDown$:BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  constructor() { }
  toggleHNavBarDropDown() {
    const currentDropDownState = this.showHNavBarDropDown$.getValue();
    const nextDropDownState = !currentDropDownState;
    this.showHNavBarDropDown$.next(nextDropDownState);
  }
}
