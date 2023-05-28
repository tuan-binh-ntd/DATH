import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { switchMap } from 'rxjs';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Order } from 'src/app/models/order-model';
import { OrderService } from 'src/app/services/order.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-view-check-order',
  templateUrl: './view-check-order.component.html',
  styleUrls: ['./view-check-order.component.less']
})
export class ViewCheckOrderComponent {
  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute,

  ){}
  data: Order;
  orderStatus: OrderStatus = OrderStatus.Pending;
  code: string = '';
  ngOnInit(){
    this.route.paramMap
    .pipe(
      switchMap(async (params) => {
        this.code = params.get('code')!;
        await this.fetchData();
      })
    )
    .subscribe();
  }
  fetchData(){
    this.orderService.getByCode(this.code).subscribe(res => {
      if(checkResponseStatus(res)) this.data = res.data;
    })
  }

}

