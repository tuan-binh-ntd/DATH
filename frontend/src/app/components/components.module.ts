import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { HeaderComponent } from './header/header.component';
import { AuthorizeComponent } from './authorize/authorize.component';
import { ButtonIconComponent } from './button-icon/button-icon.component';


@NgModule({
  declarations: [
    HeaderComponent,
    AuthorizeComponent,
    ButtonIconComponent,
  ],
  exports: [
    HeaderComponent,
    AuthorizeComponent,
    ButtonIconComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
  ],
 

})
export class ComponentsModule { }
