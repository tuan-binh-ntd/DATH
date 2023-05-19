import { NgModule } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';

import { CustomerManagementRoutingModule } from './customer-management-routing.module';
import { ComponentsModule } from '../components/components.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { CustomerHeaderComponent } from './customer-header/customer-header.component';
import { CustomerHomeComponent } from './customer-home/customer-home.component';
import { ViewProductListComponent } from './view-product-list/view-product-list.component';
import { CustomerChangeInfoComponent } from './customer-change-info/customer-change-info.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { ViewProductDetailComponent } from './view-product-list/partials/view-product-detail/view-product-detail.component';
import { SlickCarouselModule } from 'ngx-slick-carousel';
import { CustomerCartComponent } from './customer-cart/customer-cart.component';
import { ViewCartDetailComponent } from './view-cart-detail/view-cart-detail.component';

@NgModule({
  declarations: [
    CustomerLayoutComponent,
    CustomerHeaderComponent,
    CustomerHomeComponent,
    ViewProductListComponent,
    CustomerChangeInfoComponent,
    ViewProductDetailComponent,
    CustomerCartComponent,
    ViewCartDetailComponent,
  ],
  imports: [
    CommonModule,
    CustomerManagementRoutingModule,
    ComponentsModule,
    SharedModule,
    ScrollingModule,
    InfiniteScrollModule,
    SlickCarouselModule ,
  ],
})
export class CustomerManagementModule { } 
platformBrowserDynamic().bootstrapModule(CustomerManagementModule);