import { ChangeDetectorRef, Component, Input } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { Shop } from 'src/app/models/shop.model';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { OrderService } from 'src/app/services/order.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-order-for-admin-drawer',
  templateUrl: './order-for-admin-drawer.component.html',
  styleUrls: ['./order-for-admin-drawer.component.less'],
})
export class OrderForAdminDrawerComponent extends DrawerFormBaseComponent {
  @Input() listShop: Shop[];
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    private orderService: OrderService
  ) {
    super(fb, cdr, message);
  }

  formatterPrice = (value: number): string =>
    value !== null ? `${value}đ`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '';
  parserPrice = (value: string): string => value.replace(/\đ\s?|(,*)/g, '');
  formatterPercent = (value: number): string =>
    value != null ? `${value} %` : '';
  parserPercent = (value: string): string =>
    value != null ? value.replace(' %', '') : '';
  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      code: [null, Validators.required],
      cost: [null, Validators.required],
      createDate: [null, Validators.required],
      customerName: [null, Validators.required],
      discount: [null, Validators.required],
      email: [null, Validators.required],
      shopId: [null, Validators.required],
      address: [null]
    });
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      this.orderService
        .patch(this.drawerForm.getRawValue()?.id, {shopId: this.drawerForm.getRawValue()?.shopId})
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

  override setEnableForm() {
    this.drawerForm.get('shopId')?.enable();
  }


  override patchDataToForm(data: any) {
    this.data = data;
    this.drawerForm.enable();
    this.drawerForm.patchValue(data);
  }

}
