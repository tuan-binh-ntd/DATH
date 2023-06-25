import { Component } from '@angular/core';
import { Account } from 'src/app/models/account.model';
import { Admin } from 'src/app/models/admin.model';
import { EmployeeType, UserType } from 'src/app/shared/helper';

@Component({
  selector: 'app-order-list',
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.less']
})
export class OrderListComponent {
  person: Account = JSON.parse(localStorage.getItem('user')!);

  userType = UserType

  ngOnInit(){
    this.person;
  }
}
