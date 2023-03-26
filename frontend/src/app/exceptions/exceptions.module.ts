import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ExceptionsRoutingModule } from './exceptions-routing.module';
import { SharedModule } from '../shared/shared.module';
import { UnAuthorizedComponent } from './un-authorized/un-authorized.component';
import { NotFoundComponent } from './not-found/not-found.component';


@NgModule({
  declarations: [NotFoundComponent, UnAuthorizedComponent,],
  imports: [
    CommonModule,
    ExceptionsRoutingModule,
    SharedModule
  ]
})
export class ExceptionsModule { }
