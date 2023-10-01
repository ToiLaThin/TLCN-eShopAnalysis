import { Injectable } from '@angular/core';
import jwt_decode from "jwt-decode";


@Injectable({
  providedIn: 'root'
})
export class JwtService {

  constructor() { }
  
  decode(token: string): object {
    let decodedToken : object = jwt_decode(token);   
    console.log('decoded token: ', decodedToken); 
    return decodedToken;
  }
}
