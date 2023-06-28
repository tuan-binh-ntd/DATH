import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';
import { Notification } from '../models/notification.model';
import { BehaviorSubject, take } from 'rxjs';
import { NzMessageService } from 'ng-zorro-antd/message';
import { UserType } from '../shared/helper';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  // contain notification of current user
  public notificationsSource = new BehaviorSubject<Notification[]>([]);
  notifications = this.notificationsSource.asObservable();
  // contain number of unread notifications
  public unreadNotificationNum = new BehaviorSubject<Number>(0);
  unreadNotifyNum = this.unreadNotificationNum.asObservable();
  constructor(
    private msg: NzMessageService
  ) {}

  createHubConnection(user: Account) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'notify', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('NotificationList', (notifications) => {
      this.notificationsSource.next(notifications);
    });

    this.hubConnection.on('UnreadNotificationNumber', (count) => {
      this.unreadNotifyNum.pipe(take(1)).subscribe(() => {
        this.unreadNotificationNum.next(count);
      });
    });

    if (user.type === UserType.Admin || user.type === UserType.OrderTransfer) {
      // add order when customer create order
      this.hubConnection.on(
        'NewNotification',
        (newNotification: Notification) => {
          this.notifications.pipe(take(1)).subscribe((notifications) => {
            this.notificationsSource.next([...notifications, newNotification]);
            this.msg.info(newNotification.content);
          });
        }
      );
    }

    this.hubConnection.on('ReadNotification', (readNotify: Notification) => {
      this.notifications.pipe(take(1)).subscribe((notifications) => {
        const index = notifications.findIndex((n) => n.id === readNotify.id);
        this.notificationsSource.next([
          ...notifications.slice(0, index),
          readNotify,
          ...notifications.slice(index + 1),
        ]);
      });
    });
  }

  stopHubConnection() {
    this.hubConnection.stop().catch((error) => console.log(error));
  }

  async createNotify(payload: any): Promise<any> {
    return this.hubConnection
      .invoke('CreateNotificationForAdmin', payload)
      .catch((error) => console.log(error));
  }

  async readNotify(payload: any): Promise<any> {
    return this.hubConnection
      .invoke('ReadNotification', payload)
      .catch((error) => console.log(error));
  }
}
