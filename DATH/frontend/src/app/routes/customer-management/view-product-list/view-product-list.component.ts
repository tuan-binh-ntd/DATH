import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, TemplateRef, ViewChild } from '@angular/core';
import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMark, NzMarks } from 'ng-zorro-antd/slider';
import {
  debounceTime,
  distinctUntilChanged,
  forkJoin,
  fromEvent,
  switchMap,
} from 'rxjs';
import { PaginationInput } from 'src/app/models/pagination-input';
import { ProductCategory } from 'src/app/models/product-category.model';
import { Product } from 'src/app/models/product.model';
import { Specification } from 'src/app/models/specification.model';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { ProductService } from 'src/app/services/product.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-view-product-list',
  templateUrl: './view-product-list.component.html',
  styleUrls: ['./view-product-list.component.less'],
  //changeDetection: ChangeDetectionStrategy.OnPush
})
export class ViewProductListComponent {
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService
  ) {}
  @ViewChild("search") search!: ElementRef;
  paginationParam: PaginationInput = {
    pageNum: 1,
    pageSize: 10,
    totalPage: 0,
    totalCount: 0,
  };
  listTextSearch: string = '';
  type: string | null = '';
  categoryId: number = 0;
  isLoading: boolean = false;

  selectedItemIds: string[] = [];
  listCategory: ProductCategory[] = [];
  listSpecification: Specification[] = [];
  listOfData: Product[] = [];
  listColor: Specification[] = [];
  listCapacity: Specification[] = [];
  marks: NzMarks = {
    0: '10000000',
    100: '50000000',
  };
  formatter(value: any) {
    return value.toLocaleString('vi-VN', {
      style: 'currency',
      currency: 'VND',
    });
  }

  ngOnInit() {
    this.route.paramMap
      .pipe(
        switchMap(async (params) => {
          await this.fetchProductCategories();
          this.resetList();
          this.type = params.get('type')?.toUpperCase()!;
          this.categoryId = this.listCategory.find(
            (item) => item.name?.toLowerCase() === this.type?.toLowerCase()
          )?.id!;
          if (this.categoryId) {
            await this.fetchProductCategoriesSpecification();
            this.paginationParam.pageNum = 1;
            return this.fetchData();
          }
          return null;
        })
      )
      .subscribe();

  
  }

  resetList(){
    this.selectedItemIds = [];
    this.listSpecification = [];
    this.listOfData = [];
    this.listColor = [];
    this.listCapacity = [];
    this.listTextSearch = ''
  }
 ngAfterViewInit(){
  fromEvent(this.search.nativeElement, 'change')
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((text) =>{
          this.isLoading = true;
          return this.productCategoryService.getAllByCategory(
            this.categoryId,
            1,
            this.paginationParam.pageSize,
            this.selectedItemIds.join(','),
            this.listTextSearch,
          )
        }
          
        )
      )
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.isLoading = false;
          this.listOfData = res.data.content;
        }
      });
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
      .getAllBySpecificationById(this.categoryId)
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

  fetchData() {
    this.isLoading = true;
    this.productCategoryService
      .getAllByCategory(
        this.categoryId,
        this.paginationParam.pageNum,
        this.paginationParam.pageSize,
        this.selectedItemIds.join(','),
        this.listTextSearch,
      )
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.listOfData = [...this.listOfData, ...res.data.content];
          this.isLoading = false;
          this.paginationParam.totalPage = res.data.totalPage;
        }
      });
  }

  onScroll() {
    if(this.paginationParam.pageNum < this.paginationParam.totalPage){
      this.paginationParam.pageNum++;
      this.fetchData();
    }
  }

  isSelected(id: string): boolean {
    return this.selectedItemIds.includes(id);
  }

  toggleSelection(id: string) {
    this.listOfData = [];
    const index = this.selectedItemIds.indexOf(id);
    if (index === -1) {
      this.selectedItemIds.push(id); // add item ID to selection
    } else {
      this.selectedItemIds.splice(index, 1); // remove item ID from selection
    }
    this.paginationParam.pageNum = 1 ;
    this.fetchData();
  }

  goToDetail(id: string | null){
    this.router.navigateByUrl(`product-detail/${id}`);
  }
}
