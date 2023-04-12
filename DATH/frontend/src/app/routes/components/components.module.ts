import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { ButtonIconComponent } from './button-icon/button-icon.component';
import { NgModule } from '@angular/core';
import { DrawerFormBaseComponent } from './drawer-form-base/drawer-form-base.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { LoginComponent } from './login/login.component';
import { PageGridComponent } from './page-grid/page-grid.component';


@NgModule({
  declarations: [
    HeaderComponent,
    ButtonIconComponent,
    DrawerFormBaseComponent,
    LoginComponent,
    PageGridComponent,
  ],
  exports: [
    HeaderComponent,
    ButtonIconComponent,
    DrawerFormBaseComponent,
    LoginComponent,
    PageGridComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
  ],
 

})
export class ComponentsModule { }
