import { Component, Input } from '@angular/core';
import { OrderStatus } from 'src/app/enums/order-status.enum';

@Component({
  selector: 'app-order-status',
  templateUrl: './order-status.component.html',
  styleUrls: ['./order-status.component.less']
})
export class OrderStatusComponent {
  @Input() status: OrderStatus;
  @Input() width: string = '80px';
  orderStatus = OrderStatus;
  
}
