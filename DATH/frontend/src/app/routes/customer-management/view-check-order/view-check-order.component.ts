import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap } from 'rxjs';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Order } from 'src/app/models/order-model';
import { OrderService } from 'src/app/services/order.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { CartService } from 'src/app/stores/cart/cart.service';

@Component({
  selector: 'app-view-check-order',
  templateUrl: './view-check-order.component.html',
  styleUrls: ['./view-check-order.component.less'],
})
export class ViewCheckOrderComponent {
  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute,
    private router: Router,
    private cartService: CartService
  ) {}
  data: Order;
  orderStatus = OrderStatus;

  orderStatusArray: any[] = [
    { label: 'Pending', value: OrderStatus.Pending, icon:  "fa-solid fa-spinner"  },
    { label: 'Rejected', value: OrderStatus.Rejected, icon: "fa-regular fa-circle-xmark" },
    { label: 'Preparing', value: OrderStatus.Preparing, icon: "fa-light fa-box-archive" },
    { label: 'Delivering', value: OrderStatus.Delivering, icon: "fa-light fa-truck" },
    { label: 'Received', value: OrderStatus.Received, icon: "fa-regular fa-circle-check" }
  ];
  orderStatusStepIndex: number = 0;
  code: string = '';
  ngOnInit() {
    this.route.paramMap
      .pipe(
        switchMap(async (params) => {
          this.code = params.get('code')!;
          await this.fetchData();
        })
      )
      .subscribe();
  }
  fetchData() {
    this.orderService.getByCode(this.code).subscribe((res) => {
      if (checkResponseStatus(res)){
        this.data = res.data;
        switch(this.data.status){
          case OrderStatus.Pending: this.orderStatusStepIndex = 0; break;
          case OrderStatus.Rejected: this.orderStatusStepIndex = 1; break;
          case OrderStatus.Preparing: this.orderStatusStepIndex = 2; break;
          case OrderStatus.Delivering: this.orderStatusStepIndex = 3; break;
          case OrderStatus.Received: this.orderStatusStepIndex = 4; break;
        }
      } 
    });
  }

  continue() {
    this.cartService.removeAll();
    this.router.navigateByUrl('home');
  }
}
