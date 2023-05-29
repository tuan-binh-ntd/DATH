import { Component, OnInit } from '@angular/core';
import { Account } from 'src/app/models/account.model';
import { UserType } from 'src/app/shared/helper';

@Component({
  selector: 'app-admin-layout',
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.less']
})
export class AdminLayoutComponent implements OnInit {
  isCollapsed = false;
  person: Account = JSON.parse(localStorage.getItem('user')!);
  isAdmin: boolean = this.person?.type === UserType.Admin ? true : false;

  constructor() { }

  ngOnInit(): void {
  }

  onCollapse(ev: any){
    this.isCollapsed = ev;
  }

}
