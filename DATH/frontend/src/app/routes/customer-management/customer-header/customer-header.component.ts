import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Customer } from 'src/app/models/customer.model';

@Component({
  selector: 'app-customer-header',
  templateUrl: './customer-header.component.html',
  styleUrls: ['./customer-header.component.less']
})
export class CustomerHeaderComponent {
  constructor(private msg: NzMessageService,
    private fb: FormBuilder){}
  isVisible: boolean = false;
  form!: FormGroup;

  customer!: Customer;
  ngAfterViewInit(){
    this.customer = JSON.parse(localStorage.getItem('user')!);
  }

  initForm(){
    this.form = this.fb.group({
      id: [null],
      code: [null, Validators.required],
      value: [null, Validators.required],
      specificationCategoryId: [null, Validators.required],
      description: [null],
    })
  }

  signIn(){
    this.isVisible = true;
  }

  signUp(){
    this.isVisible = true;
  }


  handleCancel(): void {
    this.isVisible = false;
  }

}
