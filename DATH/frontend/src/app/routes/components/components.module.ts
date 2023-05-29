import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { ButtonIconComponent } from './button-icon/button-icon.component';
import { NgModule } from '@angular/core';
import { DrawerFormBaseComponent } from './drawer-form-base/drawer-form-base.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { LoginComponent } from './login/login.component';
import { PageGridComponent } from './page-grid/page-grid.component';
import { ListBaseComponent } from './list-base/list-base.component';
import { ButtonBaseComponent } from './button-base/button-base.component';
import { OrderStatusComponent } from './order-status/order-status.component';


@NgModule({
  declarations: [
    HeaderComponent,
    ButtonIconComponent,
    DrawerFormBaseComponent,
    LoginComponent,
    PageGridComponent,
    ListBaseComponent,
    ButtonBaseComponent,
    OrderStatusComponent,
  ],
  exports: [
    HeaderComponent,
    ButtonIconComponent,
    DrawerFormBaseComponent,
    LoginComponent,
    PageGridComponent,
    ButtonBaseComponent,
    OrderStatusComponent,
    
  ],
  imports: [
    CommonModule,
    SharedModule,
  ],
 

})
export class ComponentsModule { }
