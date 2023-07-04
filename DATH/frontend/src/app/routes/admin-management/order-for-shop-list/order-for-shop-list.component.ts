import { PresenceService } from './../../../services/presence.service';
import {
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Account } from 'src/app/models/account.model';
import { Order } from 'src/app/models/order-model';
import { PaginationInput } from 'src/app/models/pagination-input';
import { Shipping } from 'src/app/models/shipping.model';
import { OrderService } from 'src/app/services/order.service';
import { ShippingService } from 'src/app/services/shipping.service';
import { ShopService } from 'src/app/services/shop.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { OrderForAdminListComponent } from '../order-for-admin-list/order-for-admin-list.component';

@Component({
  selector: 'app-order-for-shop-list',
  templateUrl: './order-for-shop-list.component.html',
  styleUrls: ['./order-for-shop-list.component.less'],
})
export class OrderForShopListComponent {
  @ViewChild('modalContent') modalContent: any;
  deliveryForm!: FormGroup;
  disabledDate = (value: Date): boolean => {
    if (!value) {
      return false;
    }
    return value <= new Date();
  };
  orders: Order[] = [];
  listOrderPending: Order[] = [];
  listOrderRejected: Order[] = [];
  listOrderPreparing: Order[] = [];
  listOrderDelivering: Order[] = [];
  listOrderReceived: Order[] = [];
  listShipping: Shipping[] = [];
  orderStatus = OrderStatus;
  person: Account = JSON.parse(localStorage.getItem('user')!);
  paginationParam: PaginationInput = {
    pageNum: 1,
    pageSize: 12,
    totalPage: 0,
    totalCount: 0,
  };
  constructor(
    protected shopService: ShopService,
    protected orderService: OrderService,
    protected msg: NzMessageService,
    protected modalService: NzModalService,
    protected fb: FormBuilder,
    protected shippingService: ShippingService,
    private presenceService: PresenceService
  ) {}
  ngOnInit(): void {
    this.fetchShipping();
    this.fetchData();
    this.initForm();
  }
  initForm() {
    this.deliveryForm = this.fb.group({
      shippingId: [null, Validators.required],
      estimateDate: [null, Validators.required],
    });
  }

  fetchShipping(): void {
    this.shippingService.getAll().subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.listShipping = res.data;
      }
    });
  }
  fetchData(): void {
    this.shopService
      .getOrderForStores(
        this.person.shopId ?? null,
        this.paginationParam.pageNum,
        this.paginationParam.pageSize
      )
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.orders = [...res.data.content];
          this.listOrderPending = this.orders.filter(
            (item) => item.status === OrderStatus.Pending
          );
          this.listOrderRejected = this.orders.filter(
            (item) => item.status === OrderStatus.Rejected
          );
          this.listOrderPreparing = this.orders.filter(
            (item) => item.status === OrderStatus.Preparing
          );
          this.listOrderDelivering = this.orders.filter(
            (item) => item.status === OrderStatus.Delivering
          );
          this.listOrderReceived = this.orders.filter(
            (item) => item.status === OrderStatus.Received
          );
          this.paginationParam.totalCount = res.data.totalCount;
        }
      });
  }

  pageNumChanged(event: any): void {
    this.paginationParam.pageNum = event;
    this.fetchData();
  }

  pageSizeChanged(event: any) {
    this.paginationParam.pageSize = event;
    this.fetchData();
  }

  drop(event: CdkDragDrop<Order[]>, status: OrderStatus) {
    if (event.previousContainer === event.container) {
      moveItemInArray(
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
    } else {
      let payload;
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      if (event.container.id === 'delivering') {
        this.modalService.create({
          nzTitle: 'Update',
          nzCentered: true,
          nzOkText: 'Save',
          nzCancelText: 'Cancel',
          nzContent: this.modalContent,
          nzOnOk: () => {
            for (const i in this.deliveryForm.controls) {
              this.deliveryForm.controls[i].markAsDirty();
              this.deliveryForm.controls[i].updateValueAndValidity();
            }
            if (this.deliveryForm.valid) {
              payload = { ...this.deliveryForm.getRawValue(), status: status };
              this.patchOrder(event, payload);
            }
          },
          nzOnCancel: () => {
            transferArrayItem(
              event.container.data,
              event.previousContainer.data,
              event.currentIndex,
              event.previousIndex
            );
            this.deliveryForm.reset();
          },
        });
      } else {
        this.patchOrder(event, { status: status });
      }
    }
  }

  patchOrder(event: CdkDragDrop<Order[]>, payload: any) {
    this.orderService
      .patch(event.container.data[event.currentIndex].id, payload)
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.msg.success('Change status successfully');
        }
      });
  }
}
