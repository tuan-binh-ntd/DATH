import { Component } from '@angular/core';
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
  styleUrls: ['./view-product-detail.component.less']
})
export class ViewProductDetailComponent {
  id: string = '';
  value = 3;
  tooltips = ['Terrible', 'Bad', 'Normal', 'Good', 'Wonderful'];
  selectedItemId: string = '';
  listCategory: ProductCategory[] = [];
  listSpecification: Specification[] = [];
  listColor: Specification[] = [];
  listCapacity: Specification[] = [];
  data!: Product | null;
  slideConfig = { slidesToShow: 4, slidesToScroll: 4 };
  mainImgUrl: string = '';
  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService
    ){}
  ngOnInit() {
    this.route.paramMap
      .pipe(
        switchMap(async (params) => {
          await this.fetchProductCategories();
          this.resetList();
          this.id = params.get('id')!;
          await this.fetchData();
          await this.fetchProductCategoriesSpecification();
        })
      )
      .subscribe();
  }

  resetList(){}

 async fetchData(){
   await this.productService.get(this.id).toPromise().then(res => {
      if(checkResponseStatus(res)){
        this.data = res?.data;
        this.mainImgUrl = res?.data.photos![0]?.url;
      }
    })
  }

  async fetchProductCategories() {
    await this.productCategoryService
      .getAll()
      .toPromise()
      .then((res: any) => {
        if (checkResponseStatus(res)) {
          this.listCategory = res?.data;
        }
      });
  }

  async fetchProductCategoriesSpecification() {
    await this.productCategoryService
      .getAllBySpecificationById(this.data?.productCategoryId!)
      .subscribe((res: any) => {
        if (checkResponseStatus(res)) {
          this.listSpecification = res?.data;
          this.listColor = res.data.find(
            (item: any) => item.code === 'color'
          )?.specifications;
          this.listCapacity = res.data.find(
            (item: any) => item.code === 'capacity'
          )?.specifications;
        }
      });
  }

  toggleSelection(id: string) {
    this.selectedItemId = id;
  }

  setCurrentImage(url: string){
    this.mainImgUrl = url;
  }
}
