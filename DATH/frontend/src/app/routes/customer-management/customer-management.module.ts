import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CustomerManagementRoutingModule } from './customer-management-routing.module';
import { ComponentsModule } from '../components/components.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { CustomerHeaderComponent } from './customer-header/customer-header.component';
import { CustomerHomeComponent } from './customer-home/customer-home.component';


@NgModule({
  declarations: [
    CustomerLayoutComponent,
    CustomerHeaderComponent,
    CustomerHomeComponent
  ],
  imports: [
    CommonModule,
    CustomerManagementRoutingModule,
    ComponentsModule,
    SharedModule,
  ]
})
export class CustomerManagementModule { } 
