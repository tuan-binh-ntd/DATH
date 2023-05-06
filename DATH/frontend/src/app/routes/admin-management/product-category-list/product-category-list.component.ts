import { Component, HostListener, ViewChild } from '@angular/core';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { finalize } from 'rxjs';
import { checkResponseStatus } from 'src/app/shared/helper';
import { ProductCategoryDrawerComponent } from './partials/product-category-drawer/product-category-drawer.component';
import { PaginationInput } from 'src/app/models/pagination-input';

@Component({
  selector: 'app-product-category-list',
  templateUrl: './product-category-list.component.html',
  styleUrls: ['./product-category-list.component.less']
})
export class ProductCategoryListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: ProductCategoryDrawerComponent;


  constructor(protected override msg: NzMessageService,
    private productCategoryService: ProductCategoryService,) {
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
    this.productCategoryService.getAll(this.paginationParam.pageNum, this.paginationParam.pageSize).pipe(
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
