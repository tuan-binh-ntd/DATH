import { Component, Injector, OnInit } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SpecificationService } from 'src/app/services/specification.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { ListBaseComponent } from '../../components/list-base/list-base.component';

@Component({
  selector: 'app-specification-list',
  templateUrl: './specification-list.component.html',
  styleUrls: ['./specification-list.component.less']
})
export class SpecificationListComponent extends ListBaseComponent {
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
     this.specificationService.getAll().subscribe(res => {
      if(checkResponseStatus(res)){
        this.listOfData = [...res.data];
      }
     })
   }
}
