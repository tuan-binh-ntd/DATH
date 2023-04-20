import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs';
import { SpecificationCategory } from 'src/app/models/specificationCategory.model';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { SpecificationCategoryService } from 'src/app/services/specification-category.service';
import { SpecificationService } from 'src/app/services/specification.service';
import { checkResponseStatus } from 'src/app/shared/helper';

@Component({
  selector: 'app-specification-drawer',
  templateUrl: './specification-drawer.component.html',
  styleUrls: ['./specification-drawer.component.less']
})
export class SpecificationDrawerComponent extends DrawerFormBaseComponent implements OnInit {
  categories: SpecificationCategory[] = [];
  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected specificationService: SpecificationService,
    protected specificationCategoryService: SpecificationCategoryService,
  ){
    super(fb, cdr, message);
  }
  override ngOnInit(): void {
    this.initForm();
    this.fetchCategories();
  }

  fetchCategories(){
    this.specificationCategoryService.getAll().subscribe(res => {
      if(checkResponseStatus(res)){
        this.categories = res.data;
      }
    })
  }

  override checkEditForm() {
    const formValue = this.drawerForm.getRawValue();
    if (this.isEdit) {
      this.setEnableForm();
      this.titleDrawer = `Edit: ${formValue?.code}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `${formValue?.code}`;
      this.markAsUntouched();
    }
  }

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      code: [null, Validators.required],
      value: [null, Validators.required],
      specificationCategoryId: [null, Validators.required],
      description: [null],
    })
  }

override submitForm() {
    this.validateForm();
    if(this.drawerForm.valid){
      this.isLoading = true;
      if(this.mode === 'create'){
        this.specificationService.create(this.drawerForm.getRawValue()).pipe(
          finalize(() => this.isLoading = false)
        ).subscribe(res => {
          if(checkResponseStatus(res)){
            this.message.success('Create successfully');
            this.changeToDetail();
            this.onCreate.emit(res.data);
          }
        })
      }
      else{
        this.specificationService.update(this.drawerForm.value.id, this.drawerForm.getRawValue())
        .pipe(
          finalize(() => this.isLoading = false)).subscribe(res => {
          if(checkResponseStatus(res)){
            this.message.success('Update successfully');
              this.changeToDetail();
              this.onUpdate.emit(res.data);
          }
        })
      }
    }
  }

  deleteItem(){
    this.specificationService
          .delete(this.drawerForm.value.id)
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.message.success('Delete successfully');
              this.closeDrawer();
              this.onDelete.emit(res.data);
            }
          });
  }
}
