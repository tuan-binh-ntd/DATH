import { NotificationService } from './services/notification.service';
import { Component, HostListener, ViewEncapsulation } from '@angular/core';
import { ThemeService, ThemeType } from './services/theme.service';
import { Account } from './models/account.model';
import { PresenceService } from './services/presence.service';
import { OrderService } from './services/order.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less'],

})
export class AppComponent {
  users: Account;

  constructor(
    private themeService: ThemeService,
    private presenceService: PresenceService,
    private orderService: OrderService,
    private notificationService: NotificationService,
    ){}
  title = 'frontend';
  ngOnInit(){
    // this.themeService.currentTheme = window.localStorage.getItem("theme") as ThemeType;
    // if (this.themeService.currentTheme) {
    //   this.themeService.loadTheme(true);
    // }
    this.setCurrentUser();
  }

  @HostListener('window:unload', ['$event'])
  unloadHandler(event:any) {
    if (this.themeService.currentTheme) {
      window.localStorage.setItem("theme", this.themeService.currentTheme);
    }
  }


  setCurrentUser() {
    const user: Account = JSON.parse(localStorage.getItem('user'));
    if(user) {
      // connect signalr
      this.presenceService.createHubConnection(user);
      this.notificationService.createHubConnection(user);
    }
    this.orderService.createHubConnection(user);
  }
}


