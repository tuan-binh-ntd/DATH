import { Component } from '@angular/core';
import { Customer } from 'src/app/models/customer.model';

@Component({
  selector: 'app-customer-header',
  templateUrl: './customer-header.component.html',
  styleUrls: ['./customer-header.component.less']
})
export class CustomerHeaderComponent {
  customer!: Customer;
  ngAfterViewInit(){
    this.customer = JSON.parse(localStorage.getItem('user')!);
  }
}
