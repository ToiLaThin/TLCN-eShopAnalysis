import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from './auth.service';
import { AuthStatus } from '../types/auth-status.enum';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { environment as env } from 'src/environments/environment';
import * as signalR from '@microsoft/signalr';
import { CartHttpService } from './http/cart-http.service';

@Injectable()
export class SignalrService {

  hubConnection!: HubConnection;
  constructor(private toastrService: ToastrService,
              private authService: AuthService,
              private cartHttpService: CartHttpService) { 
    this.initConnection();
  }

  public stopConnection() {
    if (this.hubConnection)
    //https://learn.microsoft.com/en-us/aspnet/signalr/overview/guide-to-the-api/handling-connection-lifetime-events#clientdisconnect
    //this also called in browser event, and server will trigger OnDisconnected immediately
      this.hubConnection.stop();
  }
  public initConnection()  {        
    if (this.authService.authStatus === AuthStatus.Authenticated) {
      this.hubConnection = new HubConnectionBuilder()
                          .withUrl(`${env.NOTIFICATIONHUBROOT}`, { 
                            transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents,
                            accessTokenFactory: () => this.authService.accessToken
                          })
                          .configureLogging(LogLevel.Information)
                          .withAutomaticReconnect()
                          .build();
  
      this.hubConnection.start()
      .then(() => {
        console.log('SignalR connection started');
        this.toastrService.info('SignalR connection started', 'Success');
      })
      .catch(err => {
        console.log('Error while starting SignalR connection');
        
        this.toastrService.error(err, 'Error');
      });
  
      this.hubConnection.on('OrderStatusChanged', (message) => {
        this.toastrService.info(`Order ${message.orderId} status checkouted by 
                                      ${message.paymentMethod} at 
                                      ${message.dateCheckouted} by 
                                      ${message.paidAmount}`);
  
      });

      this.hubConnection.on('ProductModelPriceChanged', (message) => {
        this.toastrService.info(`Product ${message.productName} 's model price changed to ${message.newPrice}`);
        this.cartHttpService.updateProductModelPriceInCart(
          message.oldProductId,
          message.oldProductModelId,
          message.newProductId,
          message.newProductModelId,
          message.newPrice,
          message.newPriceOnSaleModel
        );        
      });
    }
  }
}
