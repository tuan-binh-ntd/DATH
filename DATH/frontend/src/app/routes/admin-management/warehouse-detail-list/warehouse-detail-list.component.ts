import { Component, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { NzMessageService } from 'ng-zorro-antd/message';
import { WarehouseService } from 'src/app/services/warehouse.service';
import { WarehouseDetailDrawerComponent } from './partials/warehouse-detail-drawer/warehouse-detail-drawer.component';
import { finalize } from 'rxjs';
import { EventType, checkResponseStatus } from 'src/app/shared/helper';
import { Warehouse } from 'src/app/models/warehouse.model';

@Component({
  selector: 'app-warehouse-detail-list',
  templateUrl: './warehouse-detail-list.component.html',
  styleUrls: ['./warehouse-detail-list.component.less'],
})
export class WarehouseDetailListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase')
  override drawerFormBase!: WarehouseDetailDrawerComponent;

  parentWarehouse: Warehouse = {
    id: null,
    name: null,
    shopName: null,
    shopId: null,
  };

  // get EventType for template
  public get EventType() {
    return EventType;
  }

  constructor(
    protected override msg: NzMessageService,
    private warehouseService: WarehouseService
  ) {
    super(msg);
  }
  override listOfColumn: any[] = [
    {
      name: 'Object Name',
      width: '15%',
      sortKey: 'objectName',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Product Name',
      width: '15%',
      sortKey: 'productName',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Price',
      width: '15%',
      sortKey: 'price',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Quantity',
      width: '15%',
      sortKey: 'quantity',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Actual Date',
      width: '15%',
      sortKey: 'actualDate',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Type',
      width: '15%',
      sortKey: 'type',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
  ];

  override calculateHeightBodyTable() {
    this.scrollY = `calc(100vh - 333px)`;
  }

  override fetchData(): void {
    this.warehouseService.get(0).subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.parentWarehouse = res.data;
        this.warehouseService
          .getProductsForWarehouse(
            res.data.id,
            this.paginationParam.pageNum,
            this.paginationParam.pageSize
          )
          .pipe(finalize(() => (this.isLoadingTable = false)))
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.listOfData = [...res.data.content];
              this.paginationParam.totalCount = res.data.totalCount;
            }
          });
      }
    });
  }

  // Import/Export function
  goToIE(type: EventType): void {
    this.goToCreate();
    this.drawerFormBase.drawerForm.get('type')?.setValue(type);
  }

}
