import { environment } from 'src/environments/environment';
import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Observable, Observer, finalize } from 'rxjs';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { PaymentService } from 'src/app/services/payment.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import { NzUploadFile } from 'ng-zorro-antd/upload';

const getBase64 = (file: File): Promise<string | ArrayBuffer | null> =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });

@Component({
  selector: 'app-payment-drawer',
  templateUrl: './payment-drawer.component.html',
  styleUrls: ['./payment-drawer.component.less'],
})
export class PaymentDrawerComponent extends DrawerFormBaseComponent {
  uploadUrl: string = environment.baseUrl + 'payments';
  fileList: NzUploadFile[] = [];
  previewImage: string | undefined = '';
  previewVisible = false;

  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    private paymentService: PaymentService
  ) {
    super(fb, cdr, message);
  }

  override checkEditForm() {
    const formValue = this.drawerForm.getRawValue();
    if (this.isEdit) {
      this.setEnableForm();
      this.titleDrawer = `Edit: ${formValue?.name}`;
      this.markAsTouched();
    } else {
      this.setDisableForm();
      this.titleDrawer = `${formValue?.name}`;
      this.markAsUntouched();
    }
  }

  override initForm(): void {
    this.drawerForm = this.fb.group({
      id: [null],
      name: [null, Validators.required],
    });
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      if (this.mode === 'create') {
        this.paymentService
          .create(this.drawerForm.getRawValue())
          .pipe(finalize(() => (this.isLoading = false)))
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.message.success('Create successfully');
              this.data = res.data;
              this.changeToDetail();
              this.onCreate.emit(res.data);
            }
          });
      } else {
        this.paymentService
          .update(this.drawerForm.value.id, this.drawerForm.getRawValue())
          .pipe(finalize(() => (this.isLoading = false)))
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.message.success('Update successfully');
              this.changeToDetail();
              this.onUpdate.emit(res.data);
            }
          });
      }
    }
  }

  deleteItem() {
    this.paymentService
      .delete(this.drawerForm.value.id)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.message.success('Delete successfully');
          this.closeDrawer();
          this.onDelete.emit(res.data);
        }
      });
  }

  override patchDataToForm(data: any) {
    super.patchDataToForm(data);
    this.uploadUrl += `/${data?.id}/photos`;
    if(this.isEdit) {
      this.fileList = [
        {
          uid: `${data.id}`,
          name: 'image.png',
          status: 'done',
          url: `${data.url}`,
        },
      ];
    }
  }

  beforeUpload = (
    file: NzUploadFile,
    _fileList: NzUploadFile[]
  ): Observable<boolean> =>
    new Observable((observer: Observer<boolean>) => {
      const isJpgOrPng =
        file.type === 'image/jpeg' ||
        file.type === 'image/png' ||
        file.type === 'image/webp';
      if (!isJpgOrPng) {
        this.message.error('You can only upload JPG file!');
        observer.complete();
        return;
      }
      const isLt2M = file.size! / 1024 / 1024 < 2;
      if (!isLt2M) {
        this.message.error('Image must smaller than 2MB!');
        observer.complete();
        return;
      }
      observer.next(isJpgOrPng && isLt2M);
      observer.complete();
    });

  removePhoto = (file: NzUploadFile): boolean => {
    this.paymentService.removePhoto(this.data.id).subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.fileList = [];
        this.message.success('Remove successfully');
        this.changeToDetail();
      }
    });
    return true;
  };

  handlePreview = async (file: NzUploadFile): Promise<void> => {
    if (!file.url && !file['preview']) {
      file['preview'] = await getBase64(file.originFileObj!);
    }
    this.previewImage = file.url || file['preview'];
    this.previewVisible = true;
  };
}
