import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminManagementRoutingModule } from './admin-management-routing.module';
import { SharedModule } from '../shared/shared.module';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { DashboardAdminComponent } from './dashboard-admin/dashboard-admin.component';


@NgModule({
  declarations: [
    AdminLayoutComponent,
    DashboardAdminComponent
  ],
  imports: [
    CommonModule,
    AdminManagementRoutingModule,
    SharedModule
  ]
})
export class AdminManagementModule { }
