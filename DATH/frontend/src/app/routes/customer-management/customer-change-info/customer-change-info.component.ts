import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
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
  listOrder: any[] = [];
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
    error: (error) => this.msg.error(error.message),
    complete: () => true,
  };

  changePasswordObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.changePasswordForm.reset();
        this.msg.success('Successfully');
      }
    },
    error: (error) => this.msg.error(error.message),
    complete: () => true,
  };

  addressObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        localStorage.setItem('user', JSON.stringify({...this.customer, address: res.data.address}));
        this.msg.success('Successfully');
      }
    },
    error: (error) => this.msg.error(error.message),
    complete: () => true,
  };

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private customerService: CustomerService,
    private msg: NzMessageService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.infoForm.patchValue(this.customer);
    this.infoForm.get('gender')!.setValue(this.customer.gender?.toString());
    this.getOrderHistory();
  }

  getOrderHistory(){
    this.customerService.getOrderHistory(this.infoForm.value.userId).subscribe(res => {
      if(checkResponseStatus(res)){
        this.listOrder = res.data;
      }
    })
  }

  initForm() {
    this.infoForm = this.fb.group({
      id: [null],
      firstName: [null, Validators.required],
      lastName: [null, Validators.required],
      email: [null, Validators.required],
      phone: [null, Validators.required],
      birthday: [null, Validators.required],
      gender: [null, Validators.required],
      idNumber: [null, Validators.required],
      userId: [null],
    });

    const addresses: string[] = this.customer.address ? this.customer.address!.split("|") : [];
    this.addressForm = this.fb.group({
      addresses: this.fb.array([]),
    });
    if(addresses.length > 0) {
      addresses.forEach(e => {
        const control: FormGroup = this.fb.group({
          address: [e, [Validators.required]]
        });
        this.addresses.push(control);
      });
    }
    else{
      const addressControl = this.addressForm.get('addresses') as FormArray;
      const group = this.fb.group({
        address: [null, [Validators.required]]
      })
      addressControl.push(group);
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
    if(index > 0) this.addresses.removeAt(index);
    else{
      this.addresses.at(index).get('address')?.setValue("");
    }
  }

  goToProduct(id: string){
    this.router.navigateByUrl(`/product-detail/${id}`);
  }

  goToDetail(code: string){
    this.router.navigateByUrl(`/order/${code}`);
  }
}
