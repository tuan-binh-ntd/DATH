import { Component, HostListener, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { InstallmentDrawerComponent } from './partials/installment-drawer/installment-drawer.component';
import { PaginationInput } from 'src/app/models/pagination-input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { InstallmentService } from 'src/app/services/installment.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-installment-list',
  templateUrl: './installment-list.component.html',
  styleUrls: ['./installment-list.component.less']
})
export class InstallmentListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: InstallmentDrawerComponent;
  constructor(protected override msg: NzMessageService,
    private installmentService: InstallmentService,) {
    super(msg);
  }

  override listOfColumn: any[] = [
    {
      name: 'Term',
      width: '15%',
      sortKey: 'Term',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Balance',
      width: '15%',
      sortKey: 'Balance',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    }
  ];

  override fetchData(): void {
    this.installmentService.getAll(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
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
