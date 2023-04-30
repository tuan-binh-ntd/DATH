import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Observer, finalize } from 'rxjs';
import { Shop } from 'src/app/models/shop.model';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { ShopService } from 'src/app/services/shop.service';
import { WarehouseService } from 'src/app/services/warehouse.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-warehouse-drawer',
  templateUrl: './warehouse-drawer.component.html',
  styleUrls: ['./warehouse-drawer.component.less']
})
export class WarehouseDrawerComponent extends DrawerFormBaseComponent {
  shops: Shop[] = [];

  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected warehouseService: WarehouseService,
    protected shopService: ShopService,
  ) {
    super(fb, cdr, message);
  }

  createObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.message.success('Create successfully');
        this.data = res.data;
        this.changeToDetail();
        this.onCreate.emit(res.data);
      }
    },
    error: () => this.message.error("Shop had warehouse"),
    complete: () => true,
  };

  updateObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.message.success('Update successfully');
        this.data = res.data;
        this.changeToDetail();
        this.onUpdate.emit(res.data);
      }
    },
    error: () => this.message.error("Shop had warehouse"),
    complete: () => true,
  };

  override ngOnInit(): void {
    this.initForm();
    this.fetchShops();
  }

  override patchDataToForm(data: any): void {
    super.patchDataToForm(data);
    const shopId = this.shops.find(item => item.id === data.shopId)?.id;
    this.drawerForm.get('shopId')?.setValue(shopId);
  }

  fetchShops() {
    this.shopService.getAll().subscribe(res => {
      if (checkResponseStatus(res)) {
        this.shops = res.data;
      }
    })
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
      name: [null, Validators.required],
      shopId: [null, Validators.required],
    })
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      this.isLoading = true;
      if (this.mode === 'create') {
        this.warehouseService.create(this.drawerForm.getRawValue()).pipe(
          finalize(() => this.isLoading = false)
        ).subscribe(this.createObserver)
      }
      else {
        this.warehouseService.update(this.drawerForm.value.id, this.drawerForm.getRawValue())
          .pipe(
            finalize(() => this.isLoading = false)).subscribe(this.updateObserver)
      }
    }
  }

  deleteItem() {
    this.warehouseService
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
