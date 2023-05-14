import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { Employee } from 'src/app/models/employee.model';
import { AccountService } from 'src/app/services/account.service';
import { ThemeService } from 'src/app/services/theme.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.less']
})
export class HeaderComponent implements OnInit {
  @Input() isCollapsed: boolean = false;
  @Output() emitOnCollapse = new EventEmitter();
  isVisible: boolean = false;
  employee!: Employee | any;
  isLoggedIn!: boolean | null;

  constructor(private themeService: ThemeService,
     private accountService: AccountService,
     private cookieService: CookieService,
     private router: Router) { }

  ngOnInit(): void {
    this.getCurrentUser();
    this.setCurrentUser();
  }

  setCurrentUser() {
    if(!this.cookieService.get('token')){
      this.logOut();
      this.accountService.setCurrentUser(null);
    }
    else{
      this.employee = JSON.parse(localStorage.getItem('user')!);
      this.accountService.setCurrentUser(this.employee);
    }
  }

  getCurrentUser() {
    this.accountService.currentUserSource.subscribe((res) => {
      this.isLoggedIn = !!res;
      this.employee = JSON.parse(localStorage.getItem('user')!);
    });
  }

  showModal(){
    this.isVisible = true;
  }

  onCollapse(){
    this.isCollapsed = !this.isCollapsed;
    this.emitOnCollapse.emit(this.isCollapsed);
  }

  logOut() {
    this.accountService.logOut();
    this.router.navigateByUrl('/passport/login');
    this.isLoggedIn = false;
  }

  changeTheme(){
    this.themeService.toggleTheme();
  }

  changeInfo() {
    this.router.navigateByUrl('/admin-management/employee-change-info');
  }

}
