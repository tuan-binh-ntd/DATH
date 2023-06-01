import {
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { Component } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Account } from 'src/app/models/account.model';
import { Order } from 'src/app/models/order-model';
import { PaginationInput } from 'src/app/models/pagination-input';
import { OrderService } from 'src/app/services/order.service';
import { ShopService } from 'src/app/services/shop.service';
import { checkResponseStatus, OrderStatus } from 'src/app/shared/helper';
import { OrderForShopListComponent } from '../order-for-shop-list/order-for-shop-list.component';

@Component({
  selector: 'app-order-for-admin-list',
  templateUrl: './order-for-admin-list.component.html',
  styleUrls: ['./order-for-admin-list.component.less'],
})
export class OrderForAdminListComponent {
  paginationParam: PaginationInput = {
    pageNum: 1,
    pageSize: 12,
    totalPage: 0,
    totalCount: 0,
  };
  orders: Order[] = [];
  listOrderPending: Order[] = [];
  listOrderRejected: Order[] = [];
  listOrderPreparing: Order[] = [];
  listOrderDelivering: Order[] = [];
  listOrderReceived: Order[] = [];
  orderStatus = OrderStatus;

  person: Account = JSON.parse(localStorage.getItem('user')!);
  constructor(
    protected shopService: ShopService,
    protected orderService: OrderService,
    protected msg: NzMessageService
  ) {}
  ngOnInit(): void {
    this.fetchData();
  }
  fetchData(): void {
    this.shopService
      .getOrderForAdmin(
        this.paginationParam.pageNum,
        this.paginationParam.pageSize
      )
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.orders = [...res.data.content];
          this.listOrderPending = this.orders.filter(
            (item) => item.status === OrderStatus.Pending
          );
          this.listOrderRejected = this.orders.filter(
            (item) => item.status === OrderStatus.Rejected
          );
          this.listOrderPreparing = this.orders.filter(
            (item) => item.status === OrderStatus.Preparing
          );
          this.listOrderDelivering = this.orders.filter(
            (item) => item.status === OrderStatus.Delivering
          );
          this.listOrderReceived = this.orders.filter(
            (item) => item.status === OrderStatus.Received
          );
          this.paginationParam.totalCount = res.data.totalCount;
        }
      });
  }

  pageNumChanged(event: any): void {
    this.paginationParam.pageNum = event;
    this.fetchData();
  }

  pageSizeChanged(event: any) {
    this.paginationParam.pageSize = event;
    this.fetchData();
  }

  drop(event: CdkDragDrop<Order[]>, status: OrderStatus) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    }
    else{
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex  
      );
        this.orderService
          .patch(event.previousContainer.data[event.previousIndex].id, {
            status: status,
          })
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.msg.success('Change status successfully');
            }
          });
    }
  }
}
