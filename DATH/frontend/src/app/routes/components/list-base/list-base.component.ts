import { Component, HostListener, ViewChild } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DrawerFormBaseComponent } from '../drawer-form-base/drawer-form-base.component';

@Component({
  selector: 'app-list-base',
  templateUrl: './list-base.component.html',
  styleUrls: ['./list-base.component.less']
})
export class ListBaseComponent {
  @ViewChild('drawerFormBase') drawerFormBase!: DrawerFormBaseComponent;
  listOfData: readonly any[] = [];
  listOfColumn: any[] = [];
  totalRecords: number = 0;
  currentRecordId: string = '';
  currentRecord: any;
  isLoadingTable: boolean = false;
  scrollY: string = '';
  constructor(protected msg: NzMessageService){}

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.calculateHeightBodyTable();
  }

  ngAfterViewInit() {
    this.calculateHeightBodyTable();
  }


  calculateHeightBodyTable() {
    // this.titleHeight = document.getElementsByClassName('ant-table-title')[0].clientHeight;
    // this.footerHeight = document.getElementsByClassName('ant-table-footer')[0].clientHeight;
    // this.headerHeight = document.getElementsByClassName('ant-table-thead')[0].clientHeight;
    this.scrollY = `calc(100vh - 333px)`;
  }


  
  ngOnInit(): void {
    this.fetchData();
  }

   fetchData(){}

   onSearch(){}

   onSort(direction: any, column: string){}

   goToCreate(){
    this.drawerFormBase.openDrawer(null, 'create', true);
   }

   goToDetail(data: any){
    this.drawerFormBase.openDrawer(data, 'detail', false);
   }

   onCreateItem(data: any){
    this.listOfData = [...this.listOfData, data];
   }

   onUpdateItem(data: any){
    const index = this.listOfData.findIndex(item => item.id === data.id);
    this.listOfData = [...this.listOfData.slice(0, index), ...data, this.listOfData.slice(index + 1)];
    this.listOfData = [...this.listOfData];
   }

   onDeleteItem(data: any){
    const index = this.listOfData.findIndex(item => item.id === data.id);
    this.listOfData = [...this.listOfData.slice(0, index), ...data, this.listOfData.slice(index + 1)];
    this.listOfData = [...this.listOfData];

   }

}
