import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { SpecificationService } from 'src/app/services/specification.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { SpecificationDrawerComponent } from './partials/specification-drawer/specification-drawer.component';

@Component({
  selector: 'app-specification-list',
  templateUrl: './specification-list.component.html',
  styleUrls: ['./specification-list.component.less']
})
export class SpecificationListComponent extends ListBaseComponent {
  @ViewChild('drawerFormBase') override drawerFormBase!: SpecificationDrawerComponent;
  constructor(protected override msg: NzMessageService,
  private specificationService: SpecificationService) {
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
    {
      name: 'Description',
      width: 'auto',
      sortKey: 'description',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
   ];

   override fetchData(): void {
     this.specificationService.getAll().pipe(
      finalize(() => this.isLoadingTable = false)).subscribe(res => {
      if(checkResponseStatus(res)){
        this.listOfData = [...res.data];
      }
     })
   }
}
