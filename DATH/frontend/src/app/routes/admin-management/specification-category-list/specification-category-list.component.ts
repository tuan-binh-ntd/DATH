import { Component, ViewChild } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { SpecificationCategoryService } from 'src/app/services/specification-category.service';
import { SpecificationService } from 'src/app/services/specification.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { SpecificationCategoryDrawerComponent } from './partials/specification-category-drawer/specification-category-drawer.component';

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
  
     override fetchData(): void {
      this.isLoadingTable = true;
       this.specificationCategoryService.getAll().pipe(
        finalize(() => this.isLoadingTable = false)).subscribe(res => {
        if(checkResponseStatus(res)){
          this.listOfData = [...res.data];
        }
       })
     }
}
