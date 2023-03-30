import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { HeaderComponent } from './header/header.component';
import { AuthorizeComponent } from './authorize/authorize.component';
import { ButtonIconComponent } from './button-icon/button-icon.component';
import { DrawerFormBaseComponent } from './drawer-form-base/drawer-form-base.component';


@NgModule({
  declarations: [
    HeaderComponent,
    AuthorizeComponent,
    ButtonIconComponent,
    DrawerFormBaseComponent,
  ],
  exports: [
    HeaderComponent,
    AuthorizeComponent,
    ButtonIconComponent,
    DrawerFormBaseComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
  ],
 

})
export class ComponentsModule { }
