import { NgModule } from '@angular/core';

import { AdminManagementRoutingModule } from './admin-management-routing.module';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { DashboardAdminComponent } from './dashboard-admin/dashboard-admin.component';
import { CommonModule } from '@angular/common';

import { OverlayModule } from "@angular/cdk/overlay";
import { SpecificationListComponent } from './specification-list/specification-list.component';
import { SpecificationDrawerComponent } from './specification-list/partials/specification-drawer/specification-drawer.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { ComponentsModule } from '../components/components.module';
import { AdminLoginComponent } from './admin-login/admin-login.component';

@NgModule({
  declarations: [
    AdminLayoutComponent,
    DashboardAdminComponent,
    SpecificationListComponent,
    SpecificationDrawerComponent,
    AdminLoginComponent
  ],
  imports: [
    AdminManagementRoutingModule,
    CommonModule,
    ComponentsModule,
    SharedModule,
    OverlayModule,
  ]
})
export class AdminManagementModule { }
