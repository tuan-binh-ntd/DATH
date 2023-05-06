import { Component, HostListener, ViewChild } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { SpecificationCategoryService } from 'src/app/services/specification-category.service';
import { SpecificationService } from 'src/app/services/specification.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { SpecificationCategoryDrawerComponent } from './partials/specification-category-drawer/specification-category-drawer.component';
import { PaginationInput } from 'src/app/models/pagination-input';

@Component({
  selector: 'app-specification-category-list',
  templateUrl: './specification-category-list.component.html',
  styleUrls: ['./specification-category-list.component.less']
})
export class SpecificationCategoryListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: SpecificationCategoryDrawerComponent;
 
  constructor(protected override msg: NzMessageService,
    private specificationCategoryService: SpecificationCategoryService) {
    super(msg);
  }
  override listOfColumn: any[] = [
    {
      name: 'Code',
      width: '15%',
      sortKey: 'code',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Value',
      width: '15%',
      sortKey: 'value',
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
    this.specificationCategoryService.getAll(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
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
