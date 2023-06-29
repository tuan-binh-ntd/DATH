import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { switchMap } from 'rxjs';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Order } from 'src/app/models/order-model';
import { OrderService } from 'src/app/services/order.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { CartService } from 'src/app/stores/cart/cart.service';
import { CustomerService } from 'src/app/services/customer.service';
import { Customer } from 'src/app/models/customer.model';
import { NzMessageService } from 'ng-zorro-antd/message';

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
    private cartService: CartService,
    private customerService: CustomerService,
    private msg: NzMessageService,
  ) {}
  data: any;
  orderStatus = OrderStatus;
  person: Customer = JSON.parse(localStorage.getItem('user')!);
  
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
      } 
    });
  }

  onClickReceived(){
    this.customerService.customerReceivedOrder(this.person.userId, this.data.id).subscribe(res => {
      if(checkResponseStatus(res)){
        this.msg.success("Successfully");
        this.continue();
      }
    })
  }

  continue() {
    this.cartService.removeAll();
    this.router.navigateByUrl('home');
  }
}
