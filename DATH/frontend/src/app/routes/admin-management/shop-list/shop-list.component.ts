import { Component, HostListener, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { ShopDrawerComponent } from './partials/shop-drawer/shop-drawer.component';
import { ShopService } from 'src/app/services/shop.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { checkResponseStatus } from 'src/app/shared/helper';
import { PaginationInput } from 'src/app/models/pagination-input';

@Component({
  selector: 'app-shop-list',
  templateUrl: './shop-list.component.html',
  styleUrls: ['./shop-list.component.less']
})
export class ShopListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: ShopDrawerComponent;
  
  constructor(protected override msg: NzMessageService,
    private shopService: ShopService) {
    super(msg);
  }
  override listOfColumn: any[] = [
    {
      name: 'Name',
      width: '15%',
      sortKey: 'name',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Address',
      width: '15%',
      sortKey: 'address',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    }
  ];


  override fetchData(): void {
    this.shopService.getAll(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
      finalize(() => this.isLoadingTable = false)).subscribe(res => {
        if (checkResponseStatus(res)) {
          this.listOfData = [...res.data.content];
          this.paginationParam.totalCount = res.data.totalCount;
        }
      })
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
