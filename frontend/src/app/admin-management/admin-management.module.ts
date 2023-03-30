import { NgModule } from '@angular/core';

import { AdminManagementRoutingModule } from './admin-management-routing.module';
import { SharedModule } from '../shared/shared.module';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { DashboardAdminComponent } from './dashboard-admin/dashboard-admin.component';
import { ComponentsModule } from '../components/components.module';
import { CommonModule } from '@angular/common';

import { OverlayModule } from "@angular/cdk/overlay";
import { SpecificationListComponent } from './specification-list/specification-list.component';
import { SpecificationDrawerComponent } from './specification-list/partials/specification-drawer/specification-drawer.component';

@NgModule({
  declarations: [
    AdminLayoutComponent,
    DashboardAdminComponent,
    SpecificationListComponent,
    SpecificationDrawerComponent
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
