import { ChangeDetectorRef, Component, EventEmitter, Injector, Input, OnInit, Output, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-drawer-form-base',
  templateUrl: './drawer-form-base.component.html',
  styleUrls: ['./drawer-form-base.component.less'],
})
export class DrawerFormBaseComponent  {
  @Input() isVisible: boolean = false;
  @Input() isEdit: boolean = false;
  @Input() mode: string = 'create';
  @Input() isLoading: boolean = false;
  @Input() titleDrawer: string = '';
  @Input() customFooterTpl: string | TemplateRef<{}> = "";
  @Output() onChangeEdit = new EventEmitter();
  @Output() onCloseDrawer = new EventEmitter();
  @Output() onSubmit = new EventEmitter();
  @Output() onCreate = new EventEmitter();
  @Output() onUpdate = new EventEmitter();
  @Output() onDelete = new EventEmitter();

  data: any;
  drawerForm!: FormGroup;
  constructor(
    protected fb: FormBuilder,
    protected cdr: ChangeDetectorRef,
    protected message: NzMessageService
  ) {}


  initField() {}

  initForm() {}

  ngOnInit() {
    this.initForm();
  }

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
      this.titleDrawer = `Edit: ${formValue?.code}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `${formValue?.code}`;
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
    this.resetForm();
    this.patchDataToForm(this.data);
    this.checkEditForm();
  }
  changeToUpdate() {
    this.mode = 'detail';
    this.isEdit = true;
    this.checkEditForm();
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

 submitForm() {
  this.onSubmit.emit();
 }

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
