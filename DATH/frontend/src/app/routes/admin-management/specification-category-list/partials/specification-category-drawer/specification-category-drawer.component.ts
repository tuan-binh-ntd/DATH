import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { SpecificationCategoryService } from 'src/app/services/specification-category.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-specification-category-drawer',
  templateUrl: './specification-category-drawer.component.html',
  styleUrls: ['./specification-category-drawer.component.less']
})
export class SpecificationCategoryDrawerComponent extends DrawerFormBaseComponent  {
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    private specificationCategoryService: SpecificationCategoryService
  ){
    super(fb, cdr, message);
  }
  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      code: [null, Validators.required],
      value: [null, Validators.required],
    })
  }

override submitForm() {
    this.validateForm();
    if(this.drawerForm.valid){
      this.specificationCategoryService.create(this.drawerForm.getRawValue()).subscribe(res => {
        if(checkResponseStatus(res)){
          this.message.success('Create successfully');
        }
      })
    }
  }
}
