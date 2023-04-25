import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-product-category-drawer',
  templateUrl: './product-category-drawer.component.html',
  styleUrls: ['./product-category-drawer.component.less']
})
export class ProductCategoryDrawerComponent extends DrawerFormBaseComponent {
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    private productCategoryService: ProductCategoryService
  ) {
    super(fb, cdr, message);
  }

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      name: [null, Validators.required],
    });
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      if (this.mode === 'create') {
        this.productCategoryService
          .create(this.drawerForm.getRawValue())
          .pipe(finalize(() => (this.isLoading = false)))
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.message.success('Create successfully');
              this.data = res.data;
              this.changeToDetail();
              this.onCreate.emit(res.data);
            }
          });
      } else {
        this.productCategoryService
          .update(this.drawerForm.value.id, this.drawerForm.getRawValue())
          .pipe(finalize(() => (this.isLoading = false)))
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.message.success('Update successfully');
              this.changeToDetail();
              this.onUpdate.emit(res.data);
            }
          });
      }
    }
  }

  deleteItem() {
    this.productCategoryService
      .delete(this.drawerForm.value.id)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.message.success('Delete successfully');
          this.closeDrawer();
          this.onDelete.emit(res.data);
        }
      });
  }
}
