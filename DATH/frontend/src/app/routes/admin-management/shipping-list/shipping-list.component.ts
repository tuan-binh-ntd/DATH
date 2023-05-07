import { Component, HostListener, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { ShippingDrawerComponent } from './partials/shipping-drawer/shipping-drawer.component';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ShippingService } from 'src/app/services/shipping.service';
import { finalize } from 'rxjs';
import { checkResponseStatus } from 'src/app/shared/helper';
import { PaginationInput } from 'src/app/models/pagination-input';

@Component({
  selector: 'app-shipping-list',
  templateUrl: './shipping-list.component.html',
  styleUrls: ['./shipping-list.component.less']
})
export class ShippingListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: ShippingDrawerComponent;

  constructor(protected override msg: NzMessageService,
    private shippingService: ShippingService) {
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
      name: 'Cost',
      width: '15%',
      sortKey: 'cost',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
  ];

  override fetchData(): void {
    this.shippingService.getAll(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
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
