import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { Cart } from 'src/app/stores/cart/cart.model';
import { CartQuery } from 'src/app/stores/cart/cart.query';

@Component({
  selector: 'app-customer-cart',
  templateUrl: './customer-cart.component.html',
  styleUrls: ['./customer-cart.component.less']
})
export class CustomerCartComponent {
  constructor(
    private cartQuery: CartQuery
  ){}
  cartObjects$: Observable<Cart[]> = this.cartQuery.selectAll();

  ngOnInit(){
  this.cartObjects$.subscribe(res => {
      res
    })
  }
}
