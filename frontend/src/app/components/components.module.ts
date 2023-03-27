import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { HeaderComponent } from './header/header.component';
import { AuthorizeComponent } from './authorize/authorize.component';



@NgModule({
  declarations: [
    HeaderComponent,
    AuthorizeComponent
  ],
  exports: [
    HeaderComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
  ],

})
export class ComponentsModule { }
