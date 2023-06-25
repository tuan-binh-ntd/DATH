import { Component, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { WarehouseService } from 'src/app/services/warehouse.service';
import { Warehouse } from 'src/app/models/warehouse.model';
import { StoresWarehouseDrawerComponent } from './partials/stores-warehouse-drawer/stores-warehouse-drawer.component';
import { NzMessageService } from 'ng-zorro-antd/message';
import { EventType, checkResponseStatus } from 'src/app/shared/helper';
import { finalize } from 'rxjs';
import { Account } from 'src/app/models/account.model';
import { ShopService } from 'src/app/services/shop.service';

@Component({
  selector: 'app-stores-warehouse-list',
  templateUrl: './stores-warehouse-list.component.html',
  styleUrls: ['./stores-warehouse-list.component.less']
})
export class StoresWarehouseListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase')
  override drawerFormBase!: StoresWarehouseDrawerComponent;
  employee: Account = JSON.parse(localStorage.getItem('user')!);

  parentWarehouse: Warehouse = {
    id: null,
    name: null,
    shopName: null,
    shopId: null,
  };

  // get EventType for template
  // public get EventType() {
  //   return this.EventType;
  // }

  constructor(
    protected override msg: NzMessageService,
    private warehouseService: WarehouseService,
    private shopService: ShopService
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
    this.shopService.getWarehouse(this.employee.shopId).subscribe((res) => {
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
