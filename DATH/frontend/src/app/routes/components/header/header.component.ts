import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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

  constructor(private themeService: ThemeService, private accountService: AccountService) { }

  ngOnInit(): void {
    this.getCurrentUser();
    this.setCurrentUser();
  }

  setCurrentUser() {
    this.employee = JSON.parse(localStorage.getItem('user')!);
    this.accountService.setCurrentUser(this.employee);
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

  changeTheme(){
    this.themeService.toggleTheme();
  }

}
