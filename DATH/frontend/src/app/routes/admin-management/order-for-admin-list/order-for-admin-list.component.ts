import { Component } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Account } from 'src/app/models/account.model';
import { Employee } from 'src/app/models/employee.model';
import { Shop } from 'src/app/models/shop.model';
import { ShopService } from 'src/app/services/shop.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { ListBaseComponent } from '../../components/list-base/list-base.component';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-order-for-admin-list',
  templateUrl: './order-for-admin-list.component.html',
  styleUrls: ['./order-for-admin-list.component.less'],
})
export class OrderForAdminListComponent extends ListBaseComponent {
  listShop: Shop[] = [];
  listEmployee: Employee[] = [];
  person: Account = JSON.parse(localStorage.getItem('user')!);
  constructor(
    protected override msg: NzMessageService,
    private shopService: ShopService,
    public presenceService: PresenceService
  ) {
    super(msg);
  }

  override listOfColumn: any[] = [
    {
      name: 'Code',
      width: 'auto',
      sortKey: 'code',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'CreateDate',
      width: '15%',
      sortKey: 'createDate',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-center',
    },
    {
      name: 'Shop',
      width: '20%',
      sortKey: 'shopName',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
    },
    {
      name: 'Customer',
      width: '25%',
      sortKey: 'creatorByUserName',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
    },
    {
      name: 'Cost',
      width: '15%',
      sortKey: 'cost',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
    },
  ];

  override async ngOnInit() {
    await this.fetchShop();
    this.fetchData();
  }

  async fetchShop() {
    await this.shopService
      .getAll()
      .toPromise()
      .then((res) => {
        if (checkResponseStatus(res)) {
          this.listShop = res.data;
        }
      });
  }

  override fetchData(): void {
    this.isLoadingTable = true;

    const payload = {
      pageNum: this.paginationParam.pageNum,
      pageSize: this.paginationParam.pageSize,
    };

    this.presenceService.getOrderForAdmin(payload).then((res) => {
      if (checkResponseStatus(res)) {
        this.listOfData = [...res.data.content];
        this.transformResponse();
        this.isLoadingTable = false;
        this.paginationParam.totalCount = res.data.totalCount;
      }
    });

    this.presenceService.orders.subscribe((orders) => {
      console.log(orders);
    })
  }

  override onUpdateItem(data: any) {
    const index = this.listOfData.findIndex((item) => item.id === data.id);
    this.listOfData = [
      ...this.listOfData.slice(0, index),
      data,
      ...this.listOfData.slice(index + 1),
    ];
    this.listOfData = [...this.listOfData];
    this.transformResponse();
  }

  override transformResponse(): void {
    this.listOfData.map((item) => {
      item.shopName = this.listShop.find((ele) => ele.id === item.shopId)?.name;
    });
  }
}
