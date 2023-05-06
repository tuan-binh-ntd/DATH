import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { NgZorroAntdModule } from '../routes/ng-zorro-antd/ng-zorro-antd.module';
import { EmployeeTypePipe } from './employee-type.pipe';

@NgModule({
  declarations: [
    EmployeeTypePipe
  ],
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    ReactiveFormsModule,
    RouterModule,
    NgZorroAntdModule,
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    HttpClientModule,
    NgZorroAntdModule,
    EmployeeTypePipe
  ]
})
export class SharedModule { }
