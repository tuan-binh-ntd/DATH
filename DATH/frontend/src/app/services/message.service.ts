import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Message } from '../models/message.model';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread = this.messageThreadSource.asObservable();

  constructor(
    private cookieService: CookieService,
  ) { }

  createHubConnection(otherUserName: string, taskId: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'chat?user=' + otherUserName + '&taskId=' + taskId, {
        accessTokenFactory: () => this.cookieService.get('token')!
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
