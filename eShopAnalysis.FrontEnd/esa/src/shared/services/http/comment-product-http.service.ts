import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '../auth.service';
import { IComment } from 'src/shared/models/order.interface';
import { BehaviorSubject, Subscription } from 'rxjs';
import { environment as env } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CommentProductHttpService {

  constructor(
    private http: HttpClient, 
    private _authService: AuthService        
  ) { }

  private productCommentsSub: BehaviorSubject<IComment[]> = new BehaviorSubject<IComment[]>([]);
  productComments$ = this.productCommentsSub.asObservable();

  public getProductComments(productBusinessKey: string) {
    let headers = new HttpHeaders({
      "productBusinessKey": productBusinessKey,
    });
    return this.http.get<IComment[]>(`${env.BASEURL}/api/ProductInteraction/CommentAPI/GetCommentsAboutProduct`, { headers: headers });
  }

  public GetProductComments(productBusinessKey: string) {
    this.getProductComments(productBusinessKey).subscribe((productComments) => {
      this.productCommentsSub.next(productComments);
    });
  }

  public commentProduct(userId: string, productBusinessKey: string, commentDetail: string) {
    let headers = new HttpHeaders({
      "userId": userId,
      "productBusinessKey": productBusinessKey,
      "commentDetail": commentDetail
    });
    return this.http.post(`${env.BASEURL}/api/ProductInteraction/CommentAPI/CommentProductFromUser`, "", { headers: headers });
  }
}
