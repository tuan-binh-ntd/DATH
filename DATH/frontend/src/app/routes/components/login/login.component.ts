import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})
export class LoginComponent {
  form!: FormGroup;
  passwordVisible = false;
  constructor(private fb: FormBuilder,
    private router: Router,
    private cookieService: CookieService,
    private accountService: AccountService){}
  ngOnInit(): void {
    this.initForm();
  }

  initForm(){
    this.form = this.fb.group({
      userName: [null, Validators.required],
      password: [null, Validators.required],
      remember: [null],
    })
  }

  submitForm(){
    this.accountService.signIn(this.form.value).subscribe(res => {

      this.router.navigateByUrl('admin-management/dashboard');
    })
  }

}
