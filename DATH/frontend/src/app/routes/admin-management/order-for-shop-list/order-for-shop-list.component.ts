import { Component } from '@angular/core';
import { Account } from 'src/app/models/account.model';
import { PaginationInput } from 'src/app/models/pagination-input';
import { ShopService } from 'src/app/services/shop.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-order-for-shop-list',
  templateUrl: './order-for-shop-list.component.html',
  styleUrls: ['./order-for-shop-list.component.less'],
})
export class OrderForShopListComponent {
  paginationParam: PaginationInput = {
    pageNum: 1,
    pageSize: 12,
    totalPage: 0,
    totalCount: 0,
  };

  orders: any[] = [];
  person: Account = JSON.parse(localStorage.getItem('user')!);

  constructor(private shopService: ShopService) {}
  ngOnInit(): void {
    this.fetchData();
  }
  fetchData(): void {
    this.shopService
      .getOrderForStores(
        this.person.shopId,
        this.paginationParam.pageNum,
        this.paginationParam.pageSize
      )
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.orders = [...res.data.content];
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
}
