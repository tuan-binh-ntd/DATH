import { Component } from '@angular/core';

@Component({
  selector: 'app-customer-cart',
  templateUrl: './customer-cart.component.html',
  styleUrls: ['./customer-cart.component.less']
})
export class CustomerCartComponent {
  data: any;
  ngOnInit(){
    this.data = JSON.parse(localStorage.getItem('cart')!);
  }
}
