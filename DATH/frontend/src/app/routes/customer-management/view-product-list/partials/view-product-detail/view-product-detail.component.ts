import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { switchMap } from 'rxjs';
import { ProductCategory } from 'src/app/models/product-category.model';
import { Product } from 'src/app/models/product.model';
import { Specification } from 'src/app/models/specification.model';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { ProductService } from 'src/app/services/product.service';
import { checkResponseStatus } from 'src/app/shared/helper';

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
  cartObject: any;
  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService
  ) {}
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
        }
      });
  }

  async fetchProductCategoriesSpecification() {
    this.listSpecification = this.data?.specifications!;
    if (this.listSpecification && this.listSpecification.length > 0) {
      this.listColor = this.listSpecification.filter(
        (item: any) => item.code === 'color'
      );
      this.listCapacity = this.listSpecification.filter(
        (item: any) => item.code === 'capacity'
      );
      //Initialize selection
      this.onChangeColor(this.listColor[0]?.id);
      this.onChangeCapacity(this.listCapacity[0]);
    }
  }

  onChangeColor(id: string) {
    this.selectedColor = id;
  }

  onChangeCapacity(item: Specification) {
    if (item) this.selectedCapacity = item.value;
  }

  setCurrentImage(url: string) {
    this.mainImgUrl = url;
  }

  addToCart() {
    const carts = JSON.parse(localStorage.getItem('cart')!);
    carts.push(this.data);
    localStorage.setItem('cart', JSON.stringify(carts));
  }
}
