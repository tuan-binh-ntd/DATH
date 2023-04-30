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
import { SpecificationCategoryListComponent } from './specification-category-list/specification-category-list.component';
import { SpecificationCategoryDrawerComponent } from './specification-category-list/partials/specification-category-drawer/specification-category-drawer.component';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductCategoryListComponent } from './product-category-list/product-category-list.component';
import { PromotionListComponent } from './promotion-list/promotion-list.component';
import { ShopListComponent } from './shop-list/shop-list.component';
import { ShippingListComponent } from './shipping-list/shipping-list.component';
import { ProductCategoryDrawerComponent } from './product-category-list/partials/product-category-drawer/product-category-drawer.component';
import { ProductDrawerComponent } from './product-list/partials/product-drawer/product-drawer.component';
import { PromotionDrawerComponent } from './promotion-list/partials/promotion-drawer/promotion-drawer.component';
import { ShopDrawerComponent } from './shop-list/partials/shop-drawer/shop-drawer.component';
import { ShippingDrawerComponent } from './shipping-list/partials/shipping-drawer/shipping-drawer.component';
import { RegisterEmployeeListComponent } from './register-employee-list/register-employee-list.component';
import { RegisterEmployeeDrawerComponent } from './register-employee-list/partials/register-employee-drawer/register-employee-drawer.component';
import { InstallmentListComponent } from './installment-list/installment-list.component';
import { InstallmentDrawerComponent } from './installment-list/partials/installment-drawer/installment-drawer.component';
import { WarehouseListComponent } from './warehouse-list/warehouse-list.component';
import { WarehouseDrawerComponent } from './warehouse-list/partials/warehouse-drawer/warehouse-drawer.component';

@NgModule({
  declarations: [
    AdminLayoutComponent,
    DashboardAdminComponent,
    SpecificationListComponent,
    SpecificationDrawerComponent,
    AdminLoginComponent,
    SpecificationCategoryListComponent,
    SpecificationCategoryDrawerComponent,
    ProductListComponent,
    ProductCategoryListComponent,
    PromotionListComponent,
    ShopListComponent,
    ShippingListComponent,
    ProductCategoryDrawerComponent,
    ProductDrawerComponent,
    PromotionDrawerComponent,
    ShopDrawerComponent,
    ShippingDrawerComponent,
    RegisterEmployeeListComponent,
    RegisterEmployeeDrawerComponent,
    InstallmentListComponent,
    InstallmentDrawerComponent,
    WarehouseListComponent,
    WarehouseDrawerComponent
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
