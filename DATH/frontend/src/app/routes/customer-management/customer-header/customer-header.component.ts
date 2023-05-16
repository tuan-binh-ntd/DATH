import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { NzConfigService } from 'ng-zorro-antd/core/config';
import { NzMessageService } from 'ng-zorro-antd/message';
import { CookieService } from 'ngx-cookie-service';
import { Observable, Observer, map, switchMap, timer } from 'rxjs';
import { Account } from 'src/app/models/account.model';
import { Customer } from 'src/app/models/customer.model';
import { AccountService } from 'src/app/services/account.service';
import { ThemeService } from 'src/app/services/theme.service';
import { checkResponseStatus, PASSWORD_REGEX } from 'src/app/shared/helper';
import { Cart } from 'src/app/stores/cart/cart.model';
import { CartQuery } from 'src/app/stores/cart/cart.query';

@Component({
  selector: 'app-customer-header',
  templateUrl: './customer-header.component.html',
  styleUrls: ['./customer-header.component.less'],
})
export class CustomerHeaderComponent implements OnInit {
  constructor(
    private msg: NzMessageService,
    private fb: FormBuilder,
    private accountService: AccountService,
    private cookerService: CookieService,
    private router: Router,
    private themeService: ThemeService,
    private cartQuery: CartQuery

  ) {}
  isVisible: boolean = false;
  signUpForm!: FormGroup;
  signInForm!: FormGroup;
  tabIndex: number = 0;
  customer!: Customer | any;
  isLoggedIn!: boolean | null;
  cartCount$: Observable<number> = this.cartQuery.selectCount()!;
  ngOnInit(): void {
    this.initForm();
    this.getCurrentUser();
    this.setCurrentUser();
  }

  setCurrentUser() {
    if(!this.cookerService.get('token')){
      this.logOut();
      this.accountService.setCurrentUser(null);
    }
    else{
      this.customer = JSON.parse(localStorage.getItem('user')!);
    this.accountService.setCurrentUser(this.customer);
    }
  }

  getCurrentUser() {
    this.accountService.currentUserSource.subscribe((res) => {
      this.isLoggedIn = !!res;
      this.customer = JSON.parse(localStorage.getItem('user')!);
    });
  }

  signInObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.signUpForm.reset();
        this.msg.success('Successfully!');
        this.handleCancel();
      } else {
        this.msg.error('There is an error from server');
      }
    },
    error: (error) => this.msg.error(error.error.message),
    complete: () => true,
  };

  signUpObserver: Observer<any> = {
    next: (res) => {
      if (checkResponseStatus(res)) {
        this.signUpForm.reset();
        this.msg.success('Successfully');
        this.handleCancel();
      } else {
        this.msg.error('There is an error from server');
      }
    },
    error: (error) => this.msg.error(error.error.message),
    complete: () => true,
  };

  initForm() {
    this.signUpForm = this.fb.group({
      username: [
        null,
        Validators.required,
        this.validateUsernameFromApiDebounce(),
      ],
      firstName: [null, Validators.required],
      lastName: [null, Validators.required],
      // gender: [null, Validators.required],
      // address: [null, [Validators.required]],
      // idNumber: [null, [Validators.required, Validators.pattern(IDNUMBER_REGEX)]],
      // phone: [null, [Validators.required, Validators.pattern(PHONE_REGEX)]],
      // birthday: [null, Validators.required],
      // email: [null, [Validators.required, Validators.pattern(EMAIL_REGEX)]],
      password: [
        null,
        [Validators.required, Validators.pattern(PASSWORD_REGEX)],
      ],
      checkPassword: [
        null,
        Validators.required,
        this.validateConfirmPassword(),
      ],
    });

    this.signInForm = this.fb.group({
      username: [null, Validators.required],
      password: [null, Validators.required],
      remember: [null],
    });
  }

  validateForm(form: FormGroup): void {
    for (const i in form.controls) {
      form.controls[i].markAsDirty();
      form.controls[i].updateValueAndValidity();
    }
  }

  submitSignUpForm(): void {
    this.validateForm(this.signUpForm);
    // this.signUpForm.value.gender == "1" ? this.signUpForm.value.gender = 1 : this.signUpForm.value.gender = 0;
    this.accountService
      .signUp({ ...this.signUpForm.value, isActive: true })
      // .pipe(ẻ(() => (this.isLoading = false)))
      .subscribe(this.signUpObserver);
  }

  submitSignInForm(): void {
    this.validateForm(this.signInForm);
    this.accountService
      .signIn({ ...this.signInForm.value })
      //.pipe(finalize(() => (this.isLoading = false)))
      .subscribe(this.signInObserver);
  }

  signIn() {
    this.isVisible = true;
    this.tabIndex = 0;
  }

  signUp() {
    this.isVisible = true;
    this.tabIndex = 1;
  }

  logOut() {
    this.accountService.logOut();
    // this.router.navigateByUrl('/home');
    this.isLoggedIn = false;
  }

  handleCancel(): void {
    this.isVisible = false;
    this.tabIndex = 0;
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
          let pass = this.signUpForm.get('password')?.value;
          let confirmPass = this.signUpForm.get('checkPassword')?.value;
          if (pass !== confirmPass) return { notMatch: true };
          else return null;
        })
      );
    };
  };

  changeInfo() {
    this.router.navigateByUrl('/change-info');
  }

  changeTheme(){
    this.themeService.toggleTheme();
  }
}
