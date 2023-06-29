import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { NzMessageService } from 'ng-zorro-antd/message';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';
import { UserType } from '../shared/helper';
import { BehaviorSubject, take } from 'rxjs';
import { Order, OrderForView } from '../models/order-model';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  // contain orders of current user
  public ordersSource = new BehaviorSubject<OrderForView>(null);
  orders = this.ordersSource.asObservable();

  constructor(private msg: NzMessageService) {}

  createHubConnection(user: Account) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + `presence?shopId=${user.shopId}`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('UserIsOnline', (username) => {
    });

    this.hubConnection.on('UserIsOffline', (username) => {
    });

    if (user.type === UserType.Admin || user.type === UserType.OrderTransfer) {
      this.hubConnection.on('GetOrderForAdmin', (orders) => {
        this.ordersSource.next(orders);
      });

      this.hubConnection.on('NewOrder', (newOrder: Order) => {
        if (newOrder != null) {
          this.orders.pipe(take(1)).subscribe((orders) => {
            const orderForView: OrderForView = orders;
            orderForView.content = [newOrder, ...orders.content];
            this.ordersSource.next(orderForView);
          });
        }
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
