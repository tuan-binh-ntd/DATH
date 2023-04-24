import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/guards/auth.guard';
import { CustomerAuthGuard } from 'src/app/guards/customer-auth.guard';
import { CustomerChangeInfoComponent } from './customer-change-info/customer-change-info.component';
import { CustomerHomeComponent } from './customer-home/customer-home.component';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';
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
    path: 'change-info',
    component: CustomerChangeInfoComponent,
    canActivate: [CustomerAuthGuard],

  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomerManagementRoutingModule { }
