import { Component, ViewChild } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DrawerFormBaseComponent } from '../drawer-form-base/drawer-form-base.component';

@Component({
  selector: 'app-list-base',
  templateUrl: './list-base.component.html',
  styleUrls: ['./list-base.component.less']
})
export class ListBaseComponent {
  @ViewChild('drawerFormBase') drawerFormBase!: DrawerFormBaseComponent;
  listOfData:any[] = [];
  listOfColumn: any[] = [];
  totalRecords: number = 0;
  currentRecordId: string = '';
  currentRecord: any;
  isLoadingTable: boolean = false;
  constructor(protected msg: NzMessageService){}
  ngOnInit(): void {
    this.fetchData();
  }

   fetchData(){}

   onSearch(){}

   onSort(direction: any, column: string){}

   goToCreate(){
    this.drawerFormBase.openDrawer(null, 'create', true);
   }

}
