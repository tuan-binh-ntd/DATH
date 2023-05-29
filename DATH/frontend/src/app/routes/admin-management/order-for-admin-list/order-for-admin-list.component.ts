import { Component } from '@angular/core';
import { PaginationInput } from 'src/app/models/pagination-input';
import { ShopService } from 'src/app/services/shop.service';
import { checkResponseStatus } from 'src/app/shared/helper';

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

  orders: any[] = [];

  constructor(private shopService: ShopService) {}
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
