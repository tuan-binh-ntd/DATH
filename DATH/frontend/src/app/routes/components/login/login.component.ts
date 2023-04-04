import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.less']
})
export class LoginComponent {
  form!: FormGroup;
  constructor(private fb: FormBuilder){}
  ngOnInit(): void {
    this.initForm();
  }

  initForm(){
    this.form = this.fb.group({
      userName: [null, Validators.required],
      password: [null, Validators.required],
    })
  }

  submitForm(){}

}
