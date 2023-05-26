import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/guards/auth.guard';
import { CustomerAuthGuard } from 'src/app/guards/customer-auth.guard';
import { CustomerChangeInfoComponent } from './customer-change-info/customer-change-info.component';
import { CustomerHomeComponent } from './customer-home/customer-home.component';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
import { ViewCartDetailComponent } from './view-cart-detail/view-cart-detail.component';
import { ViewCheckOrderComponent } from './view-check-order/view-check-order.component';
import { ViewProductDetailComponent } from './view-product-list/partials/view-product-detail/view-product-detail.component';
import { ViewProductListComponent } from './view-product-list/view-product-list.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
 
  {
    path: 'home',
    component: CustomerHomeComponent,
  },
  {
    path: 'product/:type',
    component: ViewProductListComponent,
  },
  {
    path: 'product-detail/:id',
    component: ViewProductDetailComponent,
  },
  {
    path: 'change-info',
    component: CustomerChangeInfoComponent,
    canActivate: [CustomerAuthGuard],
  },
  {
    path: 'cart',
    component: ViewCartDetailComponent,
  },
  {
    path: 'order/:code',
    component: ViewCheckOrderComponent,
  },
  {
    path: '**',
    component: CustomerHomeComponent,
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomerManagementRoutingModule { }
