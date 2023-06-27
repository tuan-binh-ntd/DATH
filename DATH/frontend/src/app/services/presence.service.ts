import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { NzMessageService } from 'ng-zorro-antd/message';
import { CookieService } from 'ngx-cookie-service';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';
import { UserType } from '../shared/helper';
import { BehaviorSubject } from 'rxjs';
import { Order } from '../models/order-model';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  // contain orders of current user
  public ordersSource = new BehaviorSubject<Order[]>([]);
  orders = this.ordersSource.asObservable();

  constructor(private msg: NzMessageService) {}

  createHubConnection(user: Account) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('UserIsOnline', (username) => {
      this.msg.success(username + 'has connected');
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.msg.success(username + 'has disconnected');
    });

    if (user.type === UserType.Admin || user.type === UserType.OrderTransfer) {
      this.hubConnection.on('GetOrderForAdmin', (orders) => {
        this.ordersSource.next(orders);
      });
    } else {
      this.hubConnection.on('GetOrderForShop', (orders) => {
        this.ordersSource.next(orders);
      });
    }
  }

  stopHubConnection() {
    this.hubConnection.stop().catch((error) => console.log(error));
  }

  async getOrderForAdmin(payload: any): Promise<any> {
    return this.hubConnection
      .invoke('GetOrderForAdmin', payload)
      .catch((error) => console.log(error));
  }

  async getOrderForShop(payload: any): Promise<any> {
    return this.hubConnection
      .invoke('GetOrderForShop', payload)
      .catch((error) => console.log(error));
  }
}
