import { Component, HostListener, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { RegisterEmployeeDrawerComponent } from './partials/register-employee-drawer/register-employee-drawer.component';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { checkResponseStatus } from 'src/app/shared/helper';
import { EmployeeService } from 'src/app/services/employee.service';
import { PaginationInput } from 'src/app/models/pagination-input';

@Component({
  selector: 'app-register-employee-list',
  templateUrl: './register-employee-list.component.html',
  styleUrls: ['./register-employee-list.component.less']
})
export class RegisterEmployeeListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: RegisterEmployeeDrawerComponent;
  @HostListener('window:resize', ['$event'])

  paginationParam: PaginationInput = { pageNum: 1, pageSize: 10, totalPage: 0, totalCount: 0 };
  constructor(protected override msg: NzMessageService,
    private employeeService: EmployeeService) {
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
      name: 'Code',
      width: '15%',
      sortKey: 'code',
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
    {
      name: 'Full Name',
      width: '15%',
      sortKey: 'firstName',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Join Date',
      width: '15%',
      sortKey: 'joinDate',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'IsActive',
      width: '15%',
      sortKey: 'isActive',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
  ];


  override fetchData(): void {
    this.employeeService.getAllEmployee(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
      finalize(() => this.isLoadingTable = false))
      .subscribe(res => {
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
