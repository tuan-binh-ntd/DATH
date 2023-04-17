import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { ProductCategory } from 'src/app/models/product-category.model';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { ProductService } from 'src/app/services/product.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-product-drawer',
  templateUrl: './product-drawer.component.html',
  styleUrls: ['./product-drawer.component.less']
})
export class ProductDrawerComponent extends DrawerFormBaseComponent {
  productCategories: ProductCategory[] = [];
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected productService: ProductService,
    protected productCategoryService: ProductCategoryService,
  ){
    super(fb, cdr, message);
  }
  override ngOnInit(): void {
    this.initForm();
    this.fetchCategories();
  }

  fetchCategories(){
    this.productCategoryService.getAll().subscribe(res => {
      if(checkResponseStatus(res)){
        this.productCategories =res.data;
      }
    })
  }

  override checkEditForm() {
    const formValue = this.drawerForm.getRawValue();
    if (this.isEdit) {
      this.setEnableForm();
      this.titleDrawer = `Edit: ${formValue?.code}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `${formValue?.code}`;
      this.markAsUntouched();
    }
  }

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      name: [null, Validators.required],
      price: [null, Validators.required],
      description: [null],
      specificationCategoryId: [null, Validators.required],
    })
  }

override submitForm() {
    this.validateForm();
    if(this.drawerForm.valid){
      this.isLoading = true;
      if(this.mode === 'create'){
        this.productService.create(this.drawerForm.getRawValue()).pipe(
          finalize(() => this.isLoading = false)
        ).subscribe(res => {
          if(checkResponseStatus(res)){
            this.message.success('Create successfully');
            this.changeToDetail();
            this.onCreate.emit(res.data);
          }
        })
      }
      else{
        this.productService.update(this.drawerForm.value.id, this.drawerForm.getRawValue())
        .pipe(
          finalize(() => this.isLoading = false)).subscribe(res => {
          if(checkResponseStatus(res)){
            this.message.success('Update successfully');
              this.changeToDetail();
              this.onUpdate.emit(res.data);
          }
        })
      }
    }
  }

  deleteItem(){
    this.productService
          .delete(this.drawerForm.value.id)
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.message.success('Delete successfully');
              this.closeDrawer();
              this.onDelete.emit(res.data);
            }
          });
  }
}
