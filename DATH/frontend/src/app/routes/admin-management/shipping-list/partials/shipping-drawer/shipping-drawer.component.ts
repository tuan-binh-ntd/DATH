import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { ShippingService } from 'src/app/services/shipping.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-shipping-drawer',
  templateUrl: './shipping-drawer.component.html',
  styleUrls: ['./shipping-drawer.component.less']
})
export class ShippingDrawerComponent extends DrawerFormBaseComponent {
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected shippingService: ShippingService,
  ){
    super(fb, cdr, message);
  }
  override ngOnInit(): void {
    this.initForm();
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
      code: [null, Validators.required],
      value: [null, Validators.required],
      specificationCategoryId: [null, Validators.required],
      description: [null],
    })
  }

override submitForm() {
    this.validateForm();
    if(this.drawerForm.valid){
      this.isLoading = true;
      if(this.mode === 'create'){
        this.shippingService.create(this.drawerForm.getRawValue()).pipe(
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
        this.shippingService.update(this.drawerForm.value.id, this.drawerForm.getRawValue())
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
    this.shippingService
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
