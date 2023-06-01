import { Component } from '@angular/core';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Account } from 'src/app/models/account.model';
import { Order } from 'src/app/models/order-model';
import { PaginationInput } from 'src/app/models/pagination-input';
import { ShopService } from 'src/app/services/shop.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { OrderForAdminListComponent } from '../order-for-admin-list/order-for-admin-list.component';

@Component({
  selector: 'app-order-for-shop-list',
  templateUrl: './order-for-shop-list.component.html',
  styleUrls: ['./order-for-shop-list.component.less'],
})
export class OrderForShopListComponent extends OrderForAdminListComponent{
 
  override fetchData(): void {
    this.shopService
      .getOrderForStores(
        this.person.shopId,
        this.paginationParam.pageNum,
        this.paginationParam.pageSize
      )
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.orders = [...res.data.content];
          this.listOrderPending = this.orders.filter(item => item.status === OrderStatus.Pending);
          this.listOrderRejected = this.orders.filter(item => item.status === OrderStatus.Rejected);
          this.listOrderPreparing = this.orders.filter(item => item.status === OrderStatus.Preparing);
          this.listOrderDelivering = this.orders.filter(item => item.status === OrderStatus.Delivering);
          this.listOrderPending = this.orders.filter(item => item.status === OrderStatus.Pending);
          this.paginationParam.totalCount = res.data.totalCount;
        }
      });
  }

 
}
