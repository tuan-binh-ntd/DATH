import { Component, HostListener, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { PaymentDrawerComponent } from './partials/payment-drawer/payment-drawer.component';
import { PaginationInput } from 'src/app/models/pagination-input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { PaymentService } from 'src/app/services/payment.service';
import { finalize } from 'rxjs';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-payment-list',
  templateUrl: './payment-list.component.html',
  styleUrls: ['./payment-list.component.less']
})
export class PaymentListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: PaymentDrawerComponent;


  constructor(protected override msg: NzMessageService,
    private paymentService: PaymentService,) {
    super(msg);
  }

  override listOfColumn: any[] = [
    {
      name: 'Name',
      width: '15%',
      sortKey: 'Name',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    }
  ];

  override fetchData(): void {
    this.paymentService.getAll(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
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
