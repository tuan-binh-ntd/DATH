import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Cart } from 'src/app/stores/cart/cart.model';
import { CartQuery } from 'src/app/stores/cart/cart.query';
import { CartService } from 'src/app/stores/cart/cart.service';

@Component({
  selector: 'app-customer-cart',
  templateUrl: './customer-cart.component.html',
  styleUrls: ['./customer-cart.component.less']
})
export class CustomerCartComponent {
  constructor(
    private cartQuery: CartQuery,
    private cartService: CartService,
    private router: Router
  ){}
  cartObjects$: Observable<Cart[]> = this.cartQuery.selectAll();
  objectKeys = Object.keys;
  ngOnInit(){
  this.cartObjects$.subscribe(res => {
    })
  }

  goToProduct(id: string){
    this.router.navigateByUrl(`/product-detail/${id}`);
  }
}
