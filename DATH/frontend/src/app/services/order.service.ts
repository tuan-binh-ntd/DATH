import { PresenceService } from './presence.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Order, OrderForView } from '../models/order-model';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Account } from '../models/account.model';
import { UserType } from '../shared/helper';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  baseUrl: string = environment.baseUrl + 'orders/';
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;

  constructor(
    private http: HttpClient,
    private msg: NzMessageService,
    private presenceService: PresenceService
  ) {}

  create(payload: Order): Observable<any> {
    return this.http.post(this.baseUrl, payload);
  }

  getById(id: number): Observable<any> {
    return this.http.get(this.baseUrl + id);
  }

  getByCode(code: string): Observable<any> {
    return this.http.get(this.baseUrl + 'by-code/' + code);
  }

  patch(id: string, payload): Observable<any> {
    return this.http.patch(this.baseUrl + id, payload);
  }

  createHubConnection(user: Account) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'order')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));
  }

  stopHubConnection() {
    this.hubConnection.stop().catch((error) => console.log(error));
  }

  // use when customer not logged in
  async createOrder(payload: Order): Promise<any> {
    return this.hubConnection
      .invoke('CreateOrder', payload)
      .catch((error) => console.log(error));
  }
}
