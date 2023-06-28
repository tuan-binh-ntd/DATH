import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Installment } from 'src/app/models/installment.model';
import { InstallmentService } from 'src/app/services/installment.service';
import { checkResponseStatus } from 'src/app/shared/helper';
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
    private router: Router,
    private installmentService: InstallmentService,
  ){}
  cartObjects$: Observable<Cart[]>
  objectKeys = Object.keys;
  listInstallment: Installment[] = [];

  ngOnInit(){
    this.cartObjects$  = this.cartQuery.selectAll();
    this.cartObjects$.subscribe(item => {
    })
  }
  


  goToProduct(id: string){
    this.router.navigateByUrl(`/product-detail/${id}`);
  }
}
