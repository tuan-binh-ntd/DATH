import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ExceptionsRoutingModule } from './exceptions-routing.module';
import { UnAuthorizedComponent } from './un-authorized/un-authorized.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { SharedModule } from 'src/app/shared/shared.module';


@NgModule({
  declarations: [NotFoundComponent, UnAuthorizedComponent,],
  imports: [
    CommonModule,
    ExceptionsRoutingModule,
    SharedModule
  ]
})
export class ExceptionsModule { }
