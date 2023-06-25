import { Component, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { WarehouseDrawerComponent } from './partials/warehouse-drawer/warehouse-drawer.component';
import { PaginationInput } from 'src/app/models/pagination-input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { WarehouseService } from 'src/app/services/warehouse.service';
import { finalize } from 'rxjs';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-warehouse-list',
  templateUrl: './warehouse-list.component.html',
  styleUrls: ['./warehouse-list.component.less']
})
export class WarehouseListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: WarehouseDrawerComponent;

  constructor(protected override msg: NzMessageService,
    private warehouseService: WarehouseService) {
    super(msg);
  }
  override listOfColumn: any[] = [
    {
      name: 'Shop Name',
      width: '15%',
      sortKey: 'shopName',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Name',
      width: '15%',
      sortKey: 'name',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
  ];

  override calculateHeightBodyTable() {
    this.scrollY = `calc(100vh - 333px)`;
  }

  override fetchData(): void {
    this.isLoadingTable = true;
    this.warehouseService.getAll(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
      finalize(() => this.isLoadingTable = false)).subscribe(res => {
        if (checkResponseStatus(res)) {
          this.listOfData = [...res.data.content];
          this.paginationParam.totalCount = res.data.totalCount;
        }
      })
  }

}
