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

@Component({
  selector: 'app-view-product-detail',
  templateUrl: './view-product-detail.component.html',
  styleUrls: ['./view-product-detail.component.less'],
})
export class ViewProductDetailComponent {
  id: string = '';
  value = 3;
  tooltips = ['Terrible', 'Bad', 'Normal', 'Good', 'Wonderful'];
  selectedColor: string = '';
  selectedCapacity: string = '';
  listCategory: ProductCategory[] = [];
  listSpecification: Specification[] = [];
  listColor: Specification[] = [];
  listCapacity: Specification[] = [];
  data!: Product | null;
  slideConfig = { slidesToShow: 4, slidesToScroll: 4 };
  mainImgUrl: string = '';
  //TODO Add cart model
  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService,
    private cartService: CartService,
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
  };
  cartObject$!: Subscription;
  cartObjects$ = this.cartQuery.selectAll();
  ngOnInit() {
 
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
        color: color?.code
      }
      const arr = [... this.cartObject.specifications];
      arr.splice(index, 1, obj);
      this.cartObject.specifications = [...arr];
      };
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

  setCurrentImage(url: string) {
    this.mainImgUrl = url;
  }

  addToCart() {
    this.msg.success('Added to cart');
    this.cartObject$ = this.cartQuery
      .selectEntity((item) => {
        const cart = {...item};
        cart.quantity = 1;
        cart.price = this.cartObject.cost;
        delete cart.id;
        return JSON.stringify(cart) === JSON.stringify(this.cartObject)
      })
      .pipe(take(1))
      .subscribe((res) => {
        if (res) {
          var cart = { ...res };
          cart.quantity++;
          cart.cost = cart.quantity * this.cartObject.cost;
          this.cartService.update(res.id, cart);
        } else {
          const obj = {id: Guid.create().toString(), ...this.cartObject};
          this.cartService.insert(obj);
        }
      });
        this.router.navigateByUrl(`/cart`);
  }
}
