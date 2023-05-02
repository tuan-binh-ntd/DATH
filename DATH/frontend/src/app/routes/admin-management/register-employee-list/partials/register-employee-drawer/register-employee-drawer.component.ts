import { ShopService } from 'src/app/services/shop.service';
import { DrawerFormBaseComponent } from './../../../../components/drawer-form-base/drawer-form-base.component';
import { ChangeDetectorRef, Component } from '@angular/core';
import { AccountService } from 'src/app/services/account.service';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Shop } from 'src/app/models/shop.model';
import { EMAIL_REGEX, EmployeeType, PASSWORD_REGEX, checkResponseStatus } from 'src/app/shared/helper';
import { Observable, finalize, map, switchMap, timer } from 'rxjs';
import { EmployeeService } from 'src/app/services/employee.service';

@Component({
  selector: 'app-register-employee-drawer',
  templateUrl: './register-employee-drawer.component.html',
  styleUrls: ['./register-employee-drawer.component.less']
})
export class RegisterEmployeeDrawerComponent extends DrawerFormBaseComponent {
  shops: Shop[] = [];
  types: { value: number, name: string }[] = [
    { value: EmployeeType.Sale, name: 'Sale' },
    { value: EmployeeType.Orders, name: 'Orders' },
    { value: EmployeeType.Warehouse, name: 'Warehouse' },
  ];

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

  override patchDataToForm(data: any): void {
    super.patchDataToForm(data);
    const shopId = this.shops.find(item => item.id === data.shopId)?.id;
    const gender = this.drawerForm.get('gender')?.value.toString();
    this.drawerForm.get('shopId')?.setValue(shopId);
    this.drawerForm.get('gender')?.setValue(gender);
  }

  override checkEditForm() {
    const formValue = this.drawerForm.getRawValue();
    if (this.isEdit) {
      this.setEnableForm();
      this.titleDrawer = `Edit: ${formValue?.firstName} ${formValue?.lastName}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `${formValue?.firstName} ${formValue.lastName}`;
      this.markAsUntouched();
    }
  }

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      shopId: [null, Validators.required],
      code: [null, Validators.required],
      firstName: [null, Validators.required],
      lastName: [null, Validators.required],
      gender: [null, Validators.required],
      birthday: [null, Validators.required],
      type: [null, Validators.required],
      email: [null, Validators.required, Validators.pattern(EMAIL_REGEX)],
      address: [null, Validators.required],
      username: [
        null,
        Validators.required,
        this.validateUsernameFromApiDebounce(),
      ],
      password: [
        null,
        [Validators.required, Validators.pattern(PASSWORD_REGEX)],
      ],
      checkPassword: [
        null,
        Validators.required,
        this.validateConfirmPassword(),
      ],
    })
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      this.isLoading = true;
      this.drawerForm.value.gender == "1" ? this.drawerForm.value.gender = 1 : this.drawerForm.value.gender = 0;
      if (this.mode === 'create') {
        this.accountService.register({ ...this.drawerForm.value, isActive: true }).pipe(
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

  validateUsernameFromApiDebounce = () => {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      return timer(300).pipe(
        switchMap(() =>
          this.accountService.checkUsername(control.value).pipe(
            map((res) => {
              if (res.data.invalid) {
                return null;
              }
              return {
                usernameDuplicated: true,
              };
            })
          )
        )
      );
    };
  };

  validateConfirmPassword = () => {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      return timer(300).pipe(
        map((res) => {
          let pass = this.drawerForm.get('password')?.value;
          let confirmPass = this.drawerForm.get('checkPassword')?.value;
          if (pass !== confirmPass) return { notMatch: true };
          else return null;
        })
      );
    };
  };
}
