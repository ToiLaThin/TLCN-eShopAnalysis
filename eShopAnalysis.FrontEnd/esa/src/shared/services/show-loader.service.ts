import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ShowLoaderService {
  private _isLoadingSpinner:boolean = false;
  constructor() { }

  set setIsLoadingSpinner(value:boolean){
    this._isLoadingSpinner = value;
  }

  get getIsLoadingSpinner(){
    return this._isLoadingSpinner;
  }
  
}


