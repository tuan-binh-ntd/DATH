import { Component } from '@angular/core';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Order } from 'src/app/models/order-model';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-view-check-order',
  templateUrl: './view-check-order.component.html',
  styleUrls: ['./view-check-order.component.less']
})
export class ViewCheckOrderComponent {
  constructor(
    private orderService: OrderService
  ){}
  data: Order;
  orderStatus: OrderStatus = OrderStatus.Pending;
  ngOnInit(){

  }

}

