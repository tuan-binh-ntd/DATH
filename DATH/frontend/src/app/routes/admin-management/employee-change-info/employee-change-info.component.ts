import { Employee } from './../../../models/employee.model';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Observer } from 'rxjs';
import { AccountService } from 'src/app/services/account.service';
import { EmployeeService } from 'src/app/services/employee.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-employee-change-info',
  templateUrl: './employee-change-info.component.html',
  styleUrls: ['./employee-change-info.component.less']
})
export class EmployeeChangeInfoComponent {
  selectedIndex: number = 0;
  infoForm!: FormGroup;
  changePasswordForm!: FormGroup;
  currentPasswordVisible = false;
  newPasswordVisible = false;
  confirmNewPasswordVisible = false;
  employee: Employee = JSON.parse(localStorage.getItem('user')!);

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

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private employeeService: EmployeeService,
    private msg: NzMessageService,
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.infoForm.patchValue(this.employee);
    this.infoForm.get('gender')!.setValue(this.employee.gender?.toString());
  }

  initForm() {
    this.infoForm = this.fb.group({
      firstName: [null, Validators.required],
      lastName: [null, Validators.required],
      email: [null, Validators.required],
      phone: [null, Validators.required],
      birthday: [null, Validators.required],
      gender: [null, Validators.required],
      idNumber: [null, Validators.required],
      address: [null, Validators.required],
    });

    this.changePasswordForm = this.fb.group({
      currentPassword: [null, Validators.required],
      newPassword: [null, Validators.required],
      confirmNewPassword: [null, Validators.required],
    });
  }

  submitInfoForm(): void {
    this.validateForm(this.infoForm);
    this.infoForm.value.gender == "1" ? this.infoForm.value.gender = 1 : this.infoForm.value.gender = 0;
    this.employeeService
      .update(this.employee.id!, { ...this.infoForm.value, isActive: true })
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
      .changePassword(this.employee.username!, this.changePasswordForm.value)
      .subscribe(this.changePasswordObserver);
  }
}
