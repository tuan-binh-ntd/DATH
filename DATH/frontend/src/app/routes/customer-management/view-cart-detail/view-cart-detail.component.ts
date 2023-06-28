import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import {
  debounceTime,
  distinctUntilChanged,
  fromEvent,
  Observable,
  Subscription,
  switchMap,
  take,
} from 'rxjs';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Customer } from 'src/app/models/customer.model';
import { Installment } from 'src/app/models/installment.model';
import { Order } from 'src/app/models/order-model';
import { Payment } from 'src/app/models/payment.model';
import { Product } from 'src/app/models/product.model';
import { Shop } from 'src/app/models/shop.model';
import { AccountService } from 'src/app/services/account.service';
import { CustomerService } from 'src/app/services/customer.service';
import { OrderService } from 'src/app/services/order.service';
import { PaymentService } from 'src/app/services/payment.service';
import { PromotionService } from 'src/app/services/promotion.service';
import { ShopService } from 'src/app/services/shop.service';
import {
  checkResponseStatus,
  EMAIL_REGEX,
  PHONE_REGEX,
} from 'src/app/shared/helper';
import { Cart } from 'src/app/stores/cart/cart.model';
import { CartQuery } from 'src/app/stores/cart/cart.query';
import { CartService } from 'src/app/stores/cart/cart.service';
import { StoresWarehouseListComponent } from '../../admin-management/stores-warehouse-list/stores-warehouse-list.component';

@Component({
  selector: 'app-view-cart-detail',
  templateUrl: './view-cart-detail.component.html',
  styleUrls: ['./view-cart-detail.component.less'],
})
export class ViewCartDetailComponent {
  @ViewChild('quantity') quantity!: ElementRef;
  cartObjects$ = this.cartQuery.selectAll();
  deliveryCost: number = 0;
  subTotalCost: number = 0;
  discountPercent: number = 0;
  totalCost: number = 0;

  listCart: Cart[] = [];
  selectedPayment: number = 0;
  isLoading: boolean = false;
  promotionCode: string = '';
  customer: Customer = JSON.parse(localStorage.getItem('user')!);
  listOfColumn: any[] = [
    {
      name: 'Image',
      width: '100px',
      class: 'text-left',
    },
    {
      name: 'Name',
      width: 'auto',
      sortKey: 'Name',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Price',
      width: '10%',
      sortKey: 'Price',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: 'Quantity',
      width: '15%',
      sortKey: 'Price',
      sortOrder: null,
      sortDirections: ['ascend', 'descend', null],
      class: 'text-left',
    },
    {
      name: '',
      width: '10%',
    },
  ];
  infoForm!: FormGroup;
  listShop: Shop[] = [];
  listPayment: Payment[] = [];
  listAddress: string[] = [];
  isShowPaymentMethod: boolean = false;
  constructor(
    private cartQuery: CartQuery,
    private cartService: CartService,
    private router: Router,
    private msg: NzMessageService,
    private fb: FormBuilder,
    private shopService: ShopService,
    private paymentService: PaymentService,
    private orderService: OrderService,
    private promotionService: PromotionService,
    private customerService: CustomerService,
    private accountService: AccountService
  ) {}

  async ngOnInit() {
    this.initForm();
    await this.fetchPayments();
    this.fetchShops();
    this.cartObjects$.subscribe((res) => {
      this.subTotalCost = 0;
      this.listCart = [...res];
      res.forEach((item) => {
        this.subTotalCost += item.cost;
      });
      this.totalCost = this.deliveryCost + this.subTotalCost;
    });
    this.patchValueInfoForm();
    this.checkIfHavePaymentMethod();
  }

  checkIfHavePaymentMethod(){
    this.isShowPaymentMethod = this.listCart.every(item => item.paymentId);
    if(this.isShowPaymentMethod)
    this.onChangePayment(this.listCart.find(item => item.paymentId)?.paymentId);
  }

  patchValueInfoForm(){
    this.listAddress = this.customer.address ? this.customer.address!.split("|") : [];
    if(this.customer) this.infoForm.patchValue({
      customerName: this.customer.firstName + ' ' + this.customer.lastName,
      address: this.listAddress ?.length > 0 ? this.listAddress [0] :  null,
      phone: this.customer.phone,
      email: this.customer.email,
    });
  }

  fetchShops() {
    this.shopService.getAll().subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.listShop = res.data;
      }
    });
  }

  async fetchPayments() {
    await this.paymentService
      .getAll()
      .toPromise()
      .then((res) => {
        if (checkResponseStatus(res)) {
          this.listPayment = res.data;
          const paymentId = this.listPayment.find((item) => item)?.id;
          this.selectedPayment = paymentId;
          this.infoForm.get('paymentId')?.setValue(this.selectedPayment);
        }
      });
  }

  initForm() {
    this.infoForm = this.fb.group({
      customerName: [null, Validators.required],
      address: [null],
      email: [null, [Validators.required, Validators.pattern(EMAIL_REGEX)]],
      phone: [null, [Validators.required, Validators.pattern(PHONE_REGEX)]],
      discount: [0],
      promotionId: [null],
      paymentId: [null, Validators.required],
      status: [OrderStatus.Pending],
      formal: [null],
      shopId: [null],
    });
    this.infoForm.get('formal')?.setValue('delivery');
  }

  onSort(direction: any, column: string) {}

  onChangeQuantity(id: string, value: number) {
    if (value < 10) {
      const cart = this.cartQuery.selectEntity((item) => item.id === id);
      cart.pipe(take(1)).subscribe((res) => {
        if (res) {
          const item = { ...res };
          const defaultPrice = item.cost / item.quantity;
          this.cartService.update(id, {
            quantity: value,
            cost: defaultPrice * value,
          });
        }
      });
    }
  }

  deleteRecord(id: string) {
    this.cartService.delete(id);
  }

  goToProduct(id: string) {
    this.router.navigateByUrl(`product-detail/${id}`);
  }

  onChangeFormal(ev: string) {
    switch (ev) {
      case 'store':
        this.infoForm.get('address')?.clearValidators();
        this.infoForm.get('address')?.setValue("");
        this.infoForm.get('shopId')?.addValidators(Validators.required);
        this.deliveryCost = 0;

        break;
      case 'delivery':
        this.infoForm.get('address')?.addValidators(Validators.required);
        this.infoForm.get('shopId')?.clearValidators();
        this.deliveryCost = 50000;
        break;
    }
  }

  onChangePayment(id: number) {
    if (id) {
      this.selectedPayment = id;
      this.infoForm.get('paymentId')?.setValue(this.selectedPayment);
    }
  }

  onApplyPromotion() {
    if (this.promotionCode) {
      this.promotionService
        .getByCode(this.promotionCode)
        .subscribe((res: any) => {
          if (checkResponseStatus(res)) {
            this.msg.success('Applied promotion');
            this.infoForm.get('promotionId')?.setValue(res.data.id);
            this.discountPercent = res.data.discount;
            this.totalCost = this.deliveryCost + this.subTotalCost - (this.subTotalCost
             * (this.discountPercent * 0.01));
          } else {
            this.msg.error('Promotion code not valid');
          }
        });
    }
  }

  validateForm() {
    for (const i in this.infoForm.controls) {
      this.infoForm.controls[i].markAsDirty();
      this.infoForm.controls[i].updateValueAndValidity();
    }
  }

  onCheckOut() {
    this.validateForm();
    if (this.infoForm.valid) {
      if(this.listCart?.length > 0){
        this.listCart = this.listCart.map((item) => {
          return {
            ...item,
            specificationId: item.specifications.map((item) => item.id).join(','),
          };
        });

        const payload: Order = {
          ...this.infoForm.value,
          cost: this.totalCost,
          discount: this.discountPercent,
          createDate: Date.now(),
          orderDetailInputs: this.listCart,
        };

        // this.orderService.create(payload).subscribe((res) => {
        //   if (checkResponseStatus(res)) {
        //     if(this.customer.id && !this.customer.address) this.addAddressIfNotExisted();
        //     const id = this.msg.loading('Action in progress..', { nzDuration: 0 }).messageId;
        //     setTimeout(() => {
        //       this.msg.remove(id);
        //       this.msg.success("Created order");
        //       this.router.navigateByUrl(`order/${res.data.code}`);
        //     }, 2000);
        //     // this.cartService.removeAll();
        //   }
        // });

        this.orderService.createOrder(payload).then((res) => {
          if (checkResponseStatus(res)) {
            if(this.customer.id && !this.customer.address) this.addAddressIfNotExisted();
            const id = this.msg.loading('Action in progress..', { nzDuration: 0 }).messageId;
            setTimeout(() => {
              this.msg.remove(id);
              this.msg.success("Created order");
              this.router.navigateByUrl(`order/${res.data.code}`);
            }, 1500);
            // this.cartService.removeAll();
          }
        })
      }
      else{
        this.msg.error("Your cart is empty");
      }
    }
  }

  addAddressIfNotExisted(): void {
    this.customerService
      .createAddress(this.customer.id!, {addresses: [this.infoForm.value.address]})
      .subscribe(res => {
        if(checkResponseStatus(res)){
          this.accountService.setCurrentUser(res.data, true);
        }
      });
  }
}
