import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';
import { Message } from '../models/message.model';
import * as signalR from "@microsoft/signalr"
@Injectable({
  providedIn: 'root'
})
export class ChatService {
  hubUrl = environment.hubUrl;
  private hubConnection!: signalR.HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  createHubConnection(user: Account, otherUserName: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUserName, {
        accessTokenFactory: () => user.token!
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start().catch(error => console.log(error));

      this.hubConnection.on('ReceiveMessageThread', messages => {
        this.messageThreadSource.next(messages);
      });

      this.hubConnection.on('NewMessage', message => {
        this.messageThread.pipe(take(1)).subscribe(messages => {
          this.messageThreadSource.next([...messages, message])
        })
      })
  }

  stopHubConnection() {
    if(this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  async sendMessage(payload: any) {
    return this.hubConnection.invoke('SendMessage', payload)
    .catch(error => console.log(error));
  }
}
