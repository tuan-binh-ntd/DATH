import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Observable, Observer, finalize, map, switchMap, timer } from 'rxjs';
import { Account } from 'src/app/models/account.model';
import { Customer } from 'src/app/models/customer.model';
import { AccountService } from 'src/app/services/account.service';
import { EMAIL_REGEX, IDNUMBER_REGEX, PHONE_REGEX, checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-customer-header',
  templateUrl: './customer-header.component.html',
  styleUrls: ['./customer-header.component.less']
})
export class CustomerHeaderComponent {
  constructor(private msg: NzMessageService,
    private fb: FormBuilder,
    private accountService: AccountService){}

  isVisibleSignIn: boolean = false;
  isVisibleSignUp: boolean = false;
  signUpForm!: FormGroup;
  signInForm!: FormGroup;

  customer!: Customer;

  signInObserver: Observer<Account> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.signUpForm.reset();
        this.msg.success('Successfully!');
        this.handleCancel();
      } else {
        this.msg.error("There is an error from server");
      }
    },
    error:(error) => this.msg.error(error.error.message),
    complete: () => true,
  };

  signUpObserver: Observer<Account> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.signUpForm.reset();
        this.msg.success('Successfully');
        this.handleCancel();
      }
      else {
        this.msg.error("There is an error from server");
      }
    },
    error:(error) => this.msg.error(error.error.message),
    complete: () => true,
  };

  ngAfterViewInit(){
    this.customer = JSON.parse(localStorage.getItem('user')!);
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm(){
    this.signUpForm = this.fb.group({
      username: [null, Validators.required, this.validateUsernameFromApiDebounce()],
      // firstName: [null, Validators.required],
      // lastName: [null, Validators.required],
      // gender: [null, Validators.required],
      // address: [null, [Validators.required]],
      // idNumber: [null, [Validators.required, Validators.pattern(IDNUMBER_REGEX)]],
      // phone: [null, [Validators.required, Validators.pattern(PHONE_REGEX)]],
      // birthday: [null, Validators.required],
      // email: [null, [Validators.required, Validators.pattern(EMAIL_REGEX)]],
      password: [null, Validators.required],
      checkPassword: [null, Validators.required, this.validateConfirmPassword()]
    });

    this.signInForm = this.fb.group({
      username: [null, Validators.required],
      password: [null, Validators.required],
      remember: [null],
    })
  }

  validateForm(form: FormGroup): void{
    for (const i in form.controls) {
      form.controls[i].markAsDirty();
      form.controls[i].updateValueAndValidity();
    }
  }

 
  submitSignUpForm(): void {
    this.validateForm(this.signUpForm);
    // this.signUpForm.value.gender == "1" ? this.signUpForm.value.gender = 1 : this.signUpForm.value.gender = 0;
    this.accountService.signUp({...this.signUpForm.value, isActive: true})
      //.pipe(finalize(() => (this.isLoading = false)))
      .subscribe(this.signUpObserver)
  }

  submitSignInForm(): void {
    this.validateForm(this.signInForm);
    this.accountService.signIn({...this.signInForm.value})
      //.pipe(finalize(() => (this.isLoading = false)))
      .subscribe(
        this.signInObserver
      )
  }

  signIn() {
    this.isVisibleSignIn = true;
  }

  signUp() {
    this.isVisibleSignUp = true;
  }


  handleCancel(): void {
    this.isVisibleSignIn ? this.isVisibleSignIn = false : this.isVisibleSignUp = false;
  }

  validateUsernameFromApiDebounce = () => {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      return timer(300).pipe(
        switchMap(() => this.accountService.checkUsername(control.value).pipe(
          map((res) => {
            if (res.data.invalid) {
              return null;
            }
            return {
              usernameDuplicated: true,
            };
          })
        ))
      )
    };
  };


  validateConfirmPassword = () => {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      return timer(300).pipe(
          map((res) => {
            let pass = this.signUpForm.get('password')?.value;
            let confirmPass = this.signUpForm.get('checkPassword')?.value;
            if(pass !== confirmPass) return {notMatch: true};
            else return {notMatch: false};
          })
      )
    };
  }
  confirmationValidator = (control: any): { [s: string]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.signUpForm.controls['password'].value) {
      return { confirm: true, error: true };
    }
    return {};
  };

  updateConfirmValidator(): void {
    Promise.resolve().then(() => this.signUpForm.controls['checkPassword'].updateValueAndValidity());
  }
}
