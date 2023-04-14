import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-drawer-form-base',
  templateUrl: './drawer-form-base.component.html',
  styleUrls: ['./drawer-form-base.component.less'],
})
export class DrawerFormBaseComponent implements OnInit {
  @Output() onChangeEdit = new EventEmitter();
  @Output() onCloseDrawer = new EventEmitter();
  isVisible: boolean = false;
  isEdit: boolean = false;
  data: any;
  mode: string = 'create';
  drawerForm!: FormGroup;
  titleDrawer: string = '';
  constructor(
    injector: Injector,
    protected fb: FormBuilder,
    protected cdr: ChangeDetectorRef,
    protected message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  initField() {}

  initForm() {}

  openDrawer(data: any, mode: string, isEdit: boolean) {
    this.isVisible = true;
    this.data = data;
    this.mode = mode;
    this.isEdit = isEdit;
    this.resetForm();
    switch (this.mode) {
      case 'create':
        this.initField();
        this.titleDrawer = 'Create';
        this.setEnableForm();
        break;
      case 'detail':
        this.patchDataToForm(this.data);
        this.checkEditForm();
        break;
    }
  }

  patchDataToForm(data: any) {
    this.data = data;
    this.drawerForm.patchValue(data);
  }

  resetForm() {
    this.drawerForm.reset();
  }

  checkEditForm() {
    const formValue = this.drawerForm.getRawValue();
    if (this.isEdit) {
      this.setEnableForm();
      this.titleDrawer = `Sửa: ${formValue?.name}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `Xem: ${formValue?.name}`;
      this.markAsUntouched();
    }
  }

  changeToCreate() {
    this.mode = 'create';
    this.isEdit = true;
  }
  changeToDetail() {
    this.mode = 'detail';
    this.isEdit = false;
  }
  changeToUpdate() {
    this.mode = 'detail';
    this.isEdit = true;
  }

  setEnableForm() {
    this.drawerForm.enable();
  }

  setDisableForm() {
    this.drawerForm.disable();
  }

  changeEdit() {}

  validateForm() {
    for (const i in this.drawerForm.controls) {
      this.drawerForm.controls[i].markAsDirty();
      this.drawerForm.controls[i].updateValueAndValidity();
    }
  }

  async submitForm() {}

  markAsTouched() {
    this.drawerForm.markAsTouched();
  }

  markAsUntouched() {
    this.drawerForm.markAsUntouched();
  }

  closeDrawer(){
      setTimeout(() => {
        this.isVisible = false;
        this.isEdit = false;
        this.onCloseDrawer.emit(false);
        this.onChangeEdit.emit(false);
        this.cdr.detectChanges();
      });
  }
}
