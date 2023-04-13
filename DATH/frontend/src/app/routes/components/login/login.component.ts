import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { CookieService } from 'ngx-cookie-service';
import { AccountService } from 'src/app/services/account.service';
import { checkResponseStatus } from 'src/app/shared/helper';

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
    private accountService: AccountService,
    private msg: NzMessageService){}
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
      if(checkResponseStatus(res)){
        this.cookieService.set('token', res.data.token);
        //localStorage.setItem('user', JSON.stringify(res.data));
        this.msg.success("Login successfully")
        this.router.navigateByUrl('admin-management/dashboard');
      }
      else{
        this.msg.error("Incorrect username or password")
      }
    })
  }

}
