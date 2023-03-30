import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-authorize',
  templateUrl: './authorize.component.html',
  styleUrls: ['./authorize.component.less']
})
export class AuthorizeComponent implements OnInit {
  @Input() isVisible: boolean = false;
  signInForm!: FormGroup;
  signUpForm!: FormGroup;

  constructor(private fb: FormBuilder) { }
  
  ngOnInit(): void {
    this.initForm();
  }

  initForm(){
    this.signInForm = this.fb.group({
      userName: [null, Validators.required],
      password: [null, Validators.required],
    })
  }

  submitForm(){}

}
