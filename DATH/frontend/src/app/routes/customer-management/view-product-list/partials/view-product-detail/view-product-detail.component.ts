import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { Subscription, switchMap, take } from 'rxjs';
import { Cart } from 'src/app/stores/cart/cart.model';
import { ProductCategory } from 'src/app/models/product-category.model';
import { Product } from 'src/app/models/product.model';
import { Specification } from 'src/app/models/specification.model';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { ProductService } from 'src/app/services/product.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { CartQuery } from 'src/app/stores/cart/cart.query';
import { CartService } from 'src/app/stores/cart/cart.service';
import { CartState, CartStore } from 'src/app/stores/cart/cart.store';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Subject } from '@microsoft/signalr';
import { Guid } from 'guid-typescript';
import { InstallmentService } from 'src/app/services/installment.service';
import { Installment } from 'src/app/models/installment.model';
import { Payment } from 'src/app/models/payment.model';
import { PaymentService } from 'src/app/services/payment.service';

@Component({
  selector: 'app-view-product-detail',
  templateUrl: './view-product-detail.component.html',
  styleUrls: ['./view-product-detail.component.less'],
})
export class ViewProductDetailComponent {
  id: string = '';
  value = 3;
  isShowInstallmentModal: boolean = false;
  tooltips = ['Terrible', 'Bad', 'Normal', 'Good', 'Wonderful'];
  selectedColor: string = '';
  selectedCapacity: string = '';
  selectedInstallment: number | null = null;
  listCategory: ProductCategory[] = [];
  listSpecification: Specification[] = [];
  listColor: Specification[] = [];
  listCapacity: Specification[] = [];
  listInstallment: Installment[] = [];
  listPayment: Payment[] = [];
  data!: Product | null;
  selectedPayment: number | null = null;
  costInstallment: number = 0;
  costPermonth: number = 0;
  costInterest: number = 0;
  slideConfig = { slidesToShow: 4, slidesToScroll: 4 };
  mainImgUrl: string = '';
  //TODO Add cart model
  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService,
    private cartService: CartService,
    private installmentService: InstallmentService,
    private paymentService: PaymentService,
    private cartQuery: CartQuery,
    private msg: NzMessageService,
    private router: Router,
  ) {}
  cartObject: Cart = {
    name: '',
    specifications: [],
    photo: '',
    cost: 0,
    quantity: 0,
    productId: '',
    installment: null,
    installmentId: null,
    paymentId: null,
  };
  cartObject$!: Subscription;
  cartObjects$ = this.cartQuery.selectAll();
  async ngOnInit() {
    await this.fetchInstallment();
    this.route.paramMap
      .pipe(
        switchMap(async (params) => {
          this.resetList();
          this.id = params.get('id')!;
          await this.fetchData();
          await this.fetchProductCategoriesSpecification();
        })
      )
      .subscribe();
  }

  resetList() {}

  async fetchInstallment() {
    await this.installmentService
      .getAll()
      .toPromise()
      .then((res) => {
        if (checkResponseStatus(res)) {
          this.listInstallment = res.data.sort(
            (a: any, b: any) => a.term - b.term
          );
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
          const paymentId = this.listPayment[0]?.id;
          this.selectedPayment = paymentId;
          this.onChangePayment(paymentId);
        }
      });
  }

  onChangePayment(id: number) {
    if (id) {
      this.selectedPayment = id;
      this.cartObject.paymentId = id;
    }
  }

  async fetchData() {
    await this.productService
      .get(this.id)
      .toPromise()
      .then((res) => {
        if (checkResponseStatus(res)) {
          this.data = res?.data;
          this.mainImgUrl = res?.data.photos![0]?.url;
          this.cartObject.name = res?.data.name;
          this.cartObject.photo = this.mainImgUrl;
          this.cartObject.quantity = 1;
          this.cartObject.cost = res?.data.price;
          this.cartObject.productId = res?.data.id;
        }
      });
  }

  async fetchProductCategoriesSpecification() {
    this.listSpecification = this.data?.specifications!;
    if (this.listSpecification && this.listSpecification.length > 0) {
      this.listColor = this.listSpecification.filter(
        (item: any) => item.specificationCategoryCode === 'color'
      );
      //TODO: List all capacity
      this.listCapacity = this.listSpecification.filter(
        (item: any) => item.specificationCategoryCode === 'capacity'
      );
      //Initialize selection
      this.onChangeColor(this.listColor[0]?.value);
      this.onChangeCapacity(this.listCapacity[0]);
    }
  }

  onChangeColor(value: string) {
    if (
      !this.cartObject.specifications.some((item) => {
        return Object.keys(item).includes('color');
      })
    ) {
      const color = this.listColor.find((item) => item.value === value);
      this.cartObject.specifications.push({
        id: color?.id,
        color: color?.code,
      });
    } else {
      const index = this.cartObject.specifications.findIndex((item) => {
        return Object.keys(item).includes('color');
      });
      const color = this.listColor.find((item) => item.value === value);
      const obj = {
        id: color?.id,
        color: color?.code,
      };
      const arr = [...this.cartObject.specifications];
      arr.splice(index, 1, obj);
      this.cartObject.specifications = [...arr];
    }
    this.selectedColor = value;
  }

  onChangeCapacity(item: Specification) {
    if (item) {
      this.cartObject.specifications.push({
        id: item.id,
        capacity: item.value,
      });
    }
    this.selectedCapacity = item.value;
  }

  onChangeInstallment(item: number) {
    let installment: Installment = null;
    if (item) {
      installment = this.listInstallment.find((ele) => ele.id === item);
      this.costInstallment = this.cartObject.cost - this.cartObject.cost * (installment?.balance * 0.01);
      this.costInterest = this.cartObject.cost * (installment?.interest * 0.01);
      this.costPermonth = (this.cartObject.cost +  this.costInterest) / installment?.term;
    }
    this.selectedInstallment = installment?.id;
  }

  setCurrentImage(url: string) {
    this.mainImgUrl = url;
  }

  async onOpenInstallmentModal(){
    await this.fetchPayments();
    this.isShowInstallmentModal = true;
  }

  onSaveInstallment(){
    // if (
    //   !this.cartObject.specifications.some((item) => {
    //     return Object.keys(item).includes('installment');
    //   })
    // ) {
    //   const installment = this.listInstallment.find((ele) => ele.id === this.selectedInstallment);
    //   this.cartObject.specifications.push({
    //     id: installment?.id,
    //     installment: installment?.term + ' ' + 'months',
    //   });
    // } else {
    //   const index = this.cartObject.specifications.findIndex((item) => {
    //     return Object.keys(item).includes('installment');
    //   });
    //   const installment = this.listInstallment.find((ele) => ele.id === this.selectedInstallment);
    //   const obj = {
    //     id: installment?.id,
    //     installment: installment?.term + ' ' + 'months',
    //   };
    //   const arr = [...this.cartObject.specifications];
    //   arr.splice(index, 1, obj);
    //   this.cartObject.specifications = [...arr];
    // }
    const installment = this.listInstallment.find((ele) => ele.id === this.selectedInstallment);
    this.cartObject.installment = installment;
    this.cartObject.installmentId = installment?.id;
    this.isShowInstallmentModal = false;
    this.addToCart();
  }

  addToCart() {
    this.msg.success('Added to cart');
    let cost = this.cartObject.cost;
    if(this.selectedInstallment) cost = this.costInstallment;
    this.cartObject$ = this.cartQuery
      .selectEntity((item) => {
        const cart = { ...item };
        cart.quantity = 1;
        cart.cost = cost;
        delete cart.id;
        return JSON.stringify(cart) === JSON.stringify(this.cartObject);
      })
      .pipe(take(1))
      .subscribe((res) => {
        if (res) {
          var cart = { ...res };
          cart.quantity++;
          cart.cost = cart.quantity * cost;
          this.cartService.update(res.id, cart);
        } else {
          const obj = { id: Guid.create().toString(), ...this.cartObject, cost: cost };
          this.cartService.insert(obj);
        }
      });
    this.router.navigateByUrl(`/cart`);
  }

  onClickInstallment() {
    this.isShowInstallmentModal = true;
  }
}
