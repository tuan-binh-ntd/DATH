import { ChangeDetectorRef, Component, Input } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { Product } from 'src/app/models/product.model';
import { Warehouse } from 'src/app/models/warehouse.model';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { ProductService } from 'src/app/services/product.service';
import { WarehouseService } from 'src/app/services/warehouse.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-warehouse-detail-drawer',
  templateUrl: './warehouse-detail-drawer.component.html',
  styleUrls: ['./warehouse-detail-drawer.component.less'],
})
export class WarehouseDetailDrawerComponent extends DrawerFormBaseComponent {
  products: Product[] = [];
  @Input() parentWarehouse: Warehouse = {
    id: null,
    name: null,
    shopName: null,
    shopId: null,
  };

  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected warehouseService: WarehouseService,
    protected productService: ProductService
  ) {
    super(fb, cdr, message);
  }

  override ngOnInit(): void {
    this.initForm();
    this.fetchProducts();
  }

  fetchProducts() {
    this.productService.getAll().subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.products = res.data;
      }
    });
  }

  override checkEditForm() {
    const formValue = this.drawerForm.getRawValue();
    if (this.isEdit) {
      this.setEnableForm();
      this.titleDrawer = `Edit: ${formValue?.name}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `${formValue?.name}`;
      this.markAsUntouched();
    }
  }

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      objectName: [null, Validators.required],
      productId: [null, Validators.required],
      quantity: [null, Validators.required],
      type: [null, Validators.required],
    });
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      this.isLoading = true;
      if (this.mode === 'create') {
        this.warehouseService
          .addProductToParentWarehouse(
            this.parentWarehouse.id!,
            this.drawerForm.getRawValue()
          )
          .pipe(finalize(() => (this.isLoading = false)))
          .subscribe({
            next: (res) => {
              if (checkResponseStatus(res)) {
                this.message.success('Create successfully');
                this.changeToDetail();
                this.onRefreshList.emit(res.data);
              }
            },
            error: (err) => this.message.warning(err.message),
          });
      }
    }
  }
}
