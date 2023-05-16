import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { switchMap } from 'rxjs';
import { Cart } from 'src/app/stores/cart/cart.model';
import { ProductCategory } from 'src/app/models/product-category.model';
import { Product } from 'src/app/models/product.model';
import { Specification } from 'src/app/models/specification.model';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { ProductService } from 'src/app/services/product.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { CartQuery } from 'src/app/stores/cart/cart.query';
import { CartService } from 'src/app/stores/cart/cart.service';
import { CartStore } from 'src/app/stores/cart/cart.store';
import { NzMessageService } from 'ng-zorro-antd/message';

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
  ) {}
  cartObject: Cart = {
    id: '',
    name: '',
    specifications: [],
    photo: '',
    price: '',
    quantity: 0,
  };
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
          this.cartObject.id = res?.data.id;
          this.cartObject.name = res?.data.name;
          this.cartObject.photo = this.mainImgUrl;
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
    this.cartObject.specifications.push({
      color: value
    })
    this.selectedColor = value;
  }

  onChangeCapacity(item: Specification) {
    if (item) {
      this.cartObject.specifications.push({
        capacity: item.value
      })
    }
    this.selectedCapacity = item.value;
  }

  setCurrentImage(url: string) {
    this.mainImgUrl = url;
  }

  addToCart() {
    this.msg.success("Added to cart");
    this.cartObjects$.subscribe((res) => {
      if (!res.includes(this.cartObject)){
        this.cartService.insert(this.cartObject);
      }
      else{
        this.cartService.update(this.cartObject);
      }
    });

  }
}
