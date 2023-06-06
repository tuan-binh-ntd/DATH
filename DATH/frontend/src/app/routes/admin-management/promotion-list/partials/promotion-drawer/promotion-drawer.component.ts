import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { PromotionService } from 'src/app/services/promotion.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-promotion-drawer',
  templateUrl: './promotion-drawer.component.html',
  styleUrls: ['./promotion-drawer.component.less']
})
export class PromotionDrawerComponent extends DrawerFormBaseComponent{
  formatterPercent = (value: number): string => value != null ? `${value} %` : '';
  parserPercent = (value: string): string => value != null ? value.replace(' %', '') : '';

  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected promotionServices: PromotionService,
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
      code: [null, Validators.required],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      discount: [null, Validators.required],
    })
  }

override submitForm() {
    this.validateForm();
    if(this.drawerForm.valid){
      this.isLoading = true;
      if(this.mode === 'create'){
        this.promotionServices.create(this.drawerForm.getRawValue()).pipe(
          finalize(() => this.isLoading = false)
        ).subscribe(res => {
          if(checkResponseStatus(res)){
            this.message.success('Create successfully');
            this.data = res.data;
            this.changeToDetail();
            this.onCreate.emit(res.data);
          }
        })
      }
      else{
        this.promotionServices.update(this.drawerForm.value.id, this.drawerForm.getRawValue())
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
    this.promotionServices
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
