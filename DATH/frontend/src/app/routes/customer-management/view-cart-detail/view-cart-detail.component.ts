import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { debounceTime, distinctUntilChanged, fromEvent, Observable, Subscription, switchMap, take } from 'rxjs';
import { OrderStatus } from 'src/app/enums/order-status.enum';
import { Order } from 'src/app/models/order-model';
import { Payment } from 'src/app/models/payment.model';
import { Product } from 'src/app/models/product.model';
import { Shop } from 'src/app/models/shop.model';
import { OrderService } from 'src/app/services/order.service';
import { PaymentService } from 'src/app/services/payment.service';
import { ShopService } from 'src/app/services/shop.service';
import { checkResponseStatus, EMAIL_REGEX, PHONE_REGEX } from 'src/app/shared/helper';
import { Cart } from 'src/app/stores/cart/cart.model';
import { CartQuery } from 'src/app/stores/cart/cart.query';
import { CartService } from 'src/app/stores/cart/cart.service';
import { StoresWarehouseListComponent } from '../../admin-management/stores-warehouse-list/stores-warehouse-list.component';

@Component({
  selector: 'app-view-cart-detail',
  templateUrl: './view-cart-detail.component.html',
  styleUrls: ['./view-cart-detail.component.less']
})
export class ViewCartDetailComponent {
  @ViewChild("quantity") quantity!: ElementRef;
  cartObjects$  = this.cartQuery.selectAll();
  deliveryCost: number = 250000;
  subTotalCost: number = 0;
  listCart: Cart[] = [];
  selectedPayment: number = 0;
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
    }
  ];
  infoForm!: FormGroup;
  listShop: Shop[] = [];
  listPayment: Payment[] = [];

  constructor(private cartQuery: CartQuery,
    private cartService: CartService,
    private router: Router,
    private msg: NzMessageService,
    private fb: FormBuilder,
    private shopService: ShopService,
    private paymentService: PaymentService,
    private orderService: OrderService){};


  ngOnInit(){
    this.initForm();
    this.fetchShops();
    this.fetchPayments();
   this.cartObjects$.subscribe(res => {
    this.subTotalCost = 0;
    this.listCart = res;
    res.forEach(item => {
      this.subTotalCost += item.cost;
    })
   })
  }

  fetchShops(){
    this.shopService.getAll().subscribe(res => {
      if(checkResponseStatus(res)){
        this.listShop = res.data;
      }
    })
  }

  fetchPayments(){
    this.paymentService.getAll().subscribe(res => {
      if(checkResponseStatus(res)){
        this.listPayment = res.data;
      }
    })
  }

  initForm(){
    this.infoForm = this.fb.group({
      customerName: [null, Validators.required],
      address: [null, Validators.required],
      email: [null, [Validators.required, Validators.pattern(EMAIL_REGEX)]],
      phone:  [null, [Validators.required, Validators.pattern(PHONE_REGEX)]],
      discount: [0],
      promotionId: [null, ],
      paymentId: [null, Validators.required],
      status: [OrderStatus.Pending],
      formal: [null],
      shopId: [null],
    });
    this.infoForm.get('formal')?.setValue('store');
  }

  onSort(direction: any, column: string){}

  onChangeQuantity(id: string, value: number){
    if(value < 10){
      const cart = this.cartQuery.selectEntity(item => item.id === id);
      cart.pipe(take(1)).subscribe(res => {
        if(res){
          const item = {...res};
          const defaultPrice = item.cost / item.quantity;
          this.cartService.update(id, {
            quantity: value,
            cost: defaultPrice * value,
          })
        }
      })
    }
  }

  deleteRecord(id: string){
    this.cartService.delete(id);
  }

  goToProduct(id: string){
    this.router.navigateByUrl(`product-detail/${id}`);
  }

  onChangeFormal(ev: string){
    switch(ev){
      case 'store': this.deliveryCost = 0; 

      break;
      case 'delivery': this.deliveryCost = 250000; break;
    }
  }

  onChangePayment(id: number){
    if(id){
      this.selectedPayment = id;
      this.infoForm.get('paymentId')?.setValue(id);
    } 
  }

  validateForm() {
    for (const i in this.infoForm.controls) {
      this.infoForm.controls[i].markAsDirty();
      this.infoForm.controls[i].updateValueAndValidity();
    }
  }


  onCheckOut(){
    this.validateForm();
    if(this.infoForm.valid){
      this.listCart = this.listCart.map(item => {
        return {
          ...item,
          specificationId: item.specifications.map(item => item.id).join(',')
        }
      })  
      const payload: Order = {
        ...this.infoForm.value,
        cost: this.subTotalCost + this.deliveryCost,
        createDate: Date.now(),
        orderDetailInputs: this.listCart
      }
      this.orderService.create(payload).subscribe(res => {
        if(checkResponseStatus(res)){
          // this.cartService.removeAll();
          this.router.navigateByUrl(`order/${res.data.code}`)
        }
      })
    }

  }
}
