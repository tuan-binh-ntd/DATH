import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Observer } from 'rxjs';
import { Customer } from 'src/app/models/customer.model';
import { AccountService } from 'src/app/services/account.service';
import { CustomerService } from 'src/app/services/customer.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-customer-change-info',
  templateUrl: './customer-change-info.component.html',
  styleUrls: ['./customer-change-info.component.less']
})
export class CustomerChangeInfoComponent {
  selectedIndex: number = 0;
  infoForm!: FormGroup;
  addressForm!: FormGroup;
  changePasswordForm!: FormGroup;
  currentPasswordVisible = false;
  newPasswordVisible = false;
  confirmNewPasswordVisible = false;
  customer: Customer = JSON.parse(localStorage.getItem('user')!);

  infoObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.infoForm.reset();
        this.infoForm.patchValue(res.data);
        this.infoForm.get('gender')!.setValue(res.data.gender?.toString());
        this.msg.success('Successfully');
      }
    },
    error: (error) => this.msg.error(error.error.message),
    complete: () => true,
  };

  changePasswordObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.changePasswordForm.reset();
        this.msg.success('Successfully');
      }
    },
    error: (error) => this.msg.error(error.error.message),
    complete: () => true,
  };

  addressObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        localStorage.setItem('user', JSON.stringify({...this.customer, address: res.data.address}));
        this.msg.success('Successfully');
      }
    },
    error: (error) => this.msg.error(error.error.message),
    complete: () => true,
  };

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private customerService: CustomerService,
    private msg: NzMessageService,
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.infoForm.patchValue(this.customer);
    this.infoForm.get('gender')!.setValue(this.customer.gender?.toString());
  }

  initForm() {
    this.infoForm = this.fb.group({
      firstName: [null, Validators.required],
      lastName: [null, Validators.required],
      email: [null, Validators.required],
      phone: [null, Validators.required],
      birthday: [null, Validators.required],
      gender: [null, Validators.required],
      idNumber: [null, Validators.required]
    });

    this.addressForm = this.fb.group({
      addresses: this.fb.array([]),
    });
    const addresses: string[] = this.customer.address ? this.customer.address!.split(",") : [];
    if(addresses.length > 0) {
      addresses.forEach(e => {
        const control: FormGroup = this.fb.group({
          address: [e, [Validators.required]]
        });
        this.addresses.push(control);
      });
    }

    this.changePasswordForm = this.fb.group({
      currentPassword: [null, Validators.required],
      newPassword: [null, Validators.required],
      confirmNewPassword: [null, Validators.required],
    });
  }

  submitInfoForm(): void {
    this.validateForm(this.infoForm);
    this.infoForm.value.gender == "1" ? this.infoForm.value.gender = 1 : this.infoForm.value.gender = 0;
    this.customerService
      .changeInfo(this.customer.id!, this.infoForm.value)
      .subscribe(this.infoObserver);
  }

  validateForm(form: FormGroup): void {
    for (const i in form.controls) {
      form.controls[i].markAsDirty();
      form.controls[i].updateValueAndValidity();
    }
  }

  submitChangePasswordForm(): void {
    this.validateForm(this.changePasswordForm);
    this.accountService
      .changePassword(this.customer.username!, this.changePasswordForm.value)
      .subscribe(this.changePasswordObserver);
  }

  submitAddressForm(): void {
    this.validateForm(this.addressForm);
    const payload = (this.addressForm.value.addresses as any[])?.map(e => e.address);
    this.customerService
      .createAddress(this.customer.id!, {addresses: payload})
      .subscribe(this.addressObserver);
  }

  get addresses(): FormArray {
    return this.addressForm.get('addresses') as FormArray;
  }

  addAddress(index: number) {
    const control: FormGroup = this.fb.group({
      address: [null, [Validators.required]]
    });
    this.addresses.insert(index, control);
  }

  removeAddress(index: number) {
    this.addresses.removeAt(index);
  }
}
