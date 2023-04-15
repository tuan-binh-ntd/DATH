import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { SpecificationService } from 'src/app/services/specification.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-specification-drawer',
  templateUrl: './specification-drawer.component.html',
  styleUrls: ['./specification-drawer.component.less']
})
export class SpecificationDrawerComponent extends DrawerFormBaseComponent implements OnInit {
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected specificationService: SpecificationService
  ){
    super(fb, cdr, message);
  }
  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      code: [null, Validators.required],
      value: [null, Validators.required],
      description: [null],
    })
  }

override submitForm() {
    this.validateForm();
    if(this.drawerForm.valid){
      this.specificationService.create(this.drawerForm.getRawValue()).subscribe(res => {
        if(checkResponseStatus(res)){
          this.message.success('Create successfully');
        }
      })
    }
  }
}
