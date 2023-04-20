import { ShopService } from 'src/app/services/shop.service';
import { DrawerFormBaseComponent } from './../../../../components/drawer-form-base/drawer-form-base.component';
import { ChangeDetectorRef, Component } from '@angular/core';
import { AccountService } from 'src/app/services/account.service';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Shop } from 'src/app/models/shop.model';
import { checkResponseStatus } from 'src/app/shared/helper';
import { finalize } from 'rxjs';
import { EmployeeService } from 'src/app/services/employee.service';

@Component({
  selector: 'app-register-employee-drawer',
  templateUrl: './register-employee-drawer.component.html',
  styleUrls: ['./register-employee-drawer.component.less']
})
export class RegisterEmployeeDrawerComponent extends DrawerFormBaseComponent {
  shops: Shop[] = [];
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    private accountService: AccountService,
    private employeeService: EmployeeService,
    protected shopService: ShopService,
  ) {
    super(fb, cdr, message);
  }
  override ngOnInit(): void {
    this.initForm();
    this.fetchCategories();
  }

  fetchCategories() {
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
    if (this.drawerForm.valid) {
      this.isLoading = true;
      if (this.mode === 'create') {
        this.accountService.register(this.drawerForm.getRawValue()).pipe(
          finalize(() => this.isLoading = false)
        ).subscribe(res => {
          if (checkResponseStatus(res)) {
            this.message.success('Create successfully');
            this.changeToDetail();
            this.onCreate.emit(res.data);
          }
        })
      }
      else {
        this.employeeService.update(this.drawerForm.value.id, this.drawerForm.getRawValue())
          .pipe(
            finalize(() => this.isLoading = false)).subscribe(res => {
              if (checkResponseStatus(res)) {
                this.message.success('Update successfully');
                this.changeToDetail();
                this.onUpdate.emit(res.data);
              }
            })
      }
    }
  }

  deleteItem() {
    this.employeeService
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
