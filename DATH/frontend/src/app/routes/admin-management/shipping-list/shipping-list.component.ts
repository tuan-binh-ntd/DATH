import { Component, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { ShippingDrawerComponent } from './partials/shipping-drawer/shipping-drawer.component';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ShippingService } from 'src/app/services/shipping.service';
import { finalize } from 'rxjs';
import { checkResponseStatus } from 'src/app/shared/helper';

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
      name: 'Code',
      width: '15%',
      sortKey: 'code',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Start Date',
      width: '15%',
      sortKey: 'startDate',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'End Date',
      width: '15%',
      sortKey: 'endDate',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Discount',
      width: 'auto',
      sortKey: 'discount',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
   ];

   override fetchData(): void {
     this.shippingService.getAll().pipe(
      finalize(() => this.isLoadingTable = false)).subscribe(res => {
      if(checkResponseStatus(res)){
        this.listOfData = [...res.data];
      }
     })
   }
}
