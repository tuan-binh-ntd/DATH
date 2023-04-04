import { Component, OnInit } from '@angular/core';
import { Validators } from '@angular/forms';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';

@Component({
  selector: 'app-specification-drawer',
  templateUrl: './specification-drawer.component.html',
  styleUrls: ['./specification-drawer.component.less']
})
export class SpecificationDrawerComponent extends DrawerFormBaseComponent implements OnInit {

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      code: [null, Validators.required],
      value: [null, Validators.required],
      description: [null],
    })
  }

  // override submitForm(): Promise<void> {
  //   return
  // }
}
