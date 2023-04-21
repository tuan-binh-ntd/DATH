import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { AdminLayoutComponent } from './routes/admin-management/admin-layout/admin-layout.component';
import { AdminLoginComponent } from './routes/admin-management/admin-login/admin-login.component';
import { LoginComponent } from './routes/components/login/login.component';
import { CustomerLayoutComponent } from './routes/customer-management/customer-layout/customer-layout.component';

const routes: Routes = [
  // { path: '', redirectTo: 'passport/login', pathMatch: 'full' },
  {
    path: 'passport/login',
    component: AdminLoginComponent,
  },
  // { path: 'admin-management', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'admin-management',
    component: AdminLayoutComponent,
    loadChildren: () => import('./routes/admin-management/admin-management.module').then((m) => m.AdminManagementModule),
    // canActivate: [AuthGuard],
  },
  {
    path: '',
    component: CustomerLayoutComponent,
    loadChildren: () => import('./routes/customer-management/customer-management.module').then((m) => m.CustomerManagementModule),
    // canActivate: [CheckLoadingService],
  },
  {
    path: 'exception',
    // loadChildren: './exception/exception.module#ExceptionModule',
    loadChildren: () => import('./routes/exceptions/exceptions.module').then((m) => m.ExceptionsModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
