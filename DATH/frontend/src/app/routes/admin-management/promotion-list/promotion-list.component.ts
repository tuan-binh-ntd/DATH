import { Component, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { PromotionDrawerComponent } from './partials/promotion-drawer/promotion-drawer.component';
import { NzMessageService } from 'ng-zorro-antd/message';
import { PromotionService } from 'src/app/services/promotion.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-promotion-list',
  templateUrl: './promotion-list.component.html',
  styleUrls: ['./promotion-list.component.less']
})
export class PromotionListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: PromotionDrawerComponent;
  constructor(protected override msg: NzMessageService,
  private promotionService: PromotionService) {
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
     this.promotionService.getAll().pipe(
      finalize(() => this.isLoadingTable = false)).subscribe(res => {
      if(checkResponseStatus(res)){
        this.listOfData = [...res.data];
      }
     })
   }
}
