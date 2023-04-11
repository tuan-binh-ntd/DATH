import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { AdminLayoutComponent } from './routes/admin-management/admin-layout/admin-layout.component';
import { AdminLoginComponent } from './routes/admin-management/admin-login/admin-login.component';
import { LoginComponent } from './routes/components/login/login.component';

const routes: Routes = [
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
    path: 'user-management',
    loadChildren: () => import('./user-management/user-management.module').then((m) => m.UserManagementModule),
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
