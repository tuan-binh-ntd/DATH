import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerLayoutComponent } from './customer-layout/customer-layout.component';

const routes: Routes = [
  { path: '', redirectTo: '', pathMatch: 'full' },
 
  {
    path: '',
    component: CustomerLayoutComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomerManagementRoutingModule { }
