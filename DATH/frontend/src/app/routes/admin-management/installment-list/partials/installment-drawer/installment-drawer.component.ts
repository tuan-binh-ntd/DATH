import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { InstallmentService } from 'src/app/services/installment.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-installment-drawer',
  templateUrl: './installment-drawer.component.html',
  styleUrls: ['./installment-drawer.component.less']
})
export class InstallmentDrawerComponent extends DrawerFormBaseComponent{
  formatterPercent = (value: number): string => value != null ? `${value} %` : '';
  parserPercent = (value: string): string => value != null ? value.replace(' %', '') : '';
  formatterTerm = (value: number): string => value != null ? value == 1 ? `${value} month` : `${value} months` : '';
  parserTerm = (value: string): string => value != null ? value == '1' ? value.replace(' month', '') : value.replace(' months', '') : '';

  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    private installmentService: InstallmentService
  ) {
    super(fb, cdr, message);
  }

  override checkEditForm() {
    const formValue = this.drawerForm.getRawValue();
    if (this.isEdit) {
      this.setEnableForm();
      this.titleDrawer = `Edit: ${formValue?.term}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `${formValue?.term}`;
      this.markAsUntouched();
    }
  }

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      term: [null, Validators.required],
      balance: [null, Validators.required],
    });
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      if (this.mode === 'create') {
        this.installmentService
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
        this.installmentService
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
    this.installmentService
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
