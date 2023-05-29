import { Pipe, PipeTransform } from '@angular/core';
import { OrderStatus } from './helper';

@Pipe({
  name: 'orderStatusFormatter'
})
export class OrderStatusFormatterPipe implements PipeTransform {

  transform(value: number): string {
    if(value === OrderStatus.Pending) {
      return "Pending";
    } else if (value === OrderStatus.Rejected) {
      return "Rejected";
    } else if (value === OrderStatus.Preparing) {
      return "Preparing";
    } else if (value === OrderStatus.Prepared) {
      return "Prepared";
    } else if (value === OrderStatus.Delivering) {
      return "Delivering";
    } else {
      return "Received";
    }
  }

}
