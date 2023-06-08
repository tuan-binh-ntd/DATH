import { SpecificationCategoryService } from 'src/app/services/specification-category.service';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { finalize, Observable, Observer, subscribeOn } from 'rxjs';
import { Photo } from 'src/app/models/photo.model';
import { ProductCategory } from 'src/app/models/product-category.model';
import { Specification } from 'src/app/models/specification.model';
import { DrawerFormBaseComponent } from 'src/app/routes/components/drawer-form-base/drawer-form-base.component';
import { ProductCategoryService } from 'src/app/services/product-category.service';
import { ProductService } from 'src/app/services/product.service';
import { SpecificationService } from 'src/app/services/specification.service';
import { checkResponseStatus } from 'src/app/shared/helper';
import Quill from 'quill';
import { ImageHandler } from 'ngx-quill-upload';

// or, from each individual module


const getBase64 = (file: File): Promise<string | ArrayBuffer | null> =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });
Quill.register('modules/imageHandler', ImageHandler);

@Component({
  selector: 'app-product-drawer',
  templateUrl: './product-drawer.component.html',
  styleUrls: ['./product-drawer.component.less'],
})
export class ProductDrawerComponent extends DrawerFormBaseComponent {
  @ViewChild('editor') editor!: ElementRef;
  productCategories: ProductCategory[] = [];
  specifications: Specification[] = [];
  // Upload photo properties
  fileList: NzUploadFile[] = [];
  previewImage: string | undefined = '';
  previewVisible = false;
  uploadUrl: string = '';

  // Set color when upload photo
  colors: any[] = [];
  colorForm!: FormGroup;

  // Setup quill for upload photo
  editorOptions = {
    imageResize: {
      displaySize: true
    },
  
    imageHandler: {
      upload: (file: any) => {
        return new Promise((resolve, reject) => {
          if (
            file.type === 'image/jpeg' ||
            file.type === 'image/png' ||
            file.type === 'image/jpg'
          ) {
            return this.productService
              .addPhoto(this.drawerForm.value.id, false, file, '')
              .toPromise()
              .then((res: any) => {
                if (checkResponseStatus(res)) {
                  resolve(res.data.photos[res.data.photos.length - 1]?.url);
                }
              })
              .catch((err: any) => {
                return reject('Upload Failed');
              });
          } else {
            return reject('Unsupported type image');
          }
        });
      },
    },
  };

  // preview photo
  handlePreview = async (file: NzUploadFile): Promise<void> => {
    if (!file.url && !file['preview']) {
      file['preview'] = await getBase64(file.originFileObj!);
    }
    this.previewImage = file.url || file['preview'];
    this.previewVisible = true;
  };

  // formatter functions
  formatterPrice = (value: number): string =>
    value !== null ? `${value}đ`.replace(/\B(?=(\d{3})+(?!\d))/g, ',') : '';
  parserPrice = (value: string): string => value.replace(/\đ\s?|(,*)/g, '');

  constructor(
    protected override fb: FormBuilder,
    protected override cdr: ChangeDetectorRef,
    protected override message: NzMessageService,
    protected productService: ProductService,
    protected productCategoryService: ProductCategoryService,
    protected specificationService: SpecificationService,
    private specificationCategoryService: SpecificationCategoryService
  ) {
    super(fb, cdr, message);
  }

  override ngOnInit(): void {
    this.initForm();
    this.fetchCategories();
    this.fetchSpecification();
    this.fetchColor();
  }

  fetchCategories() {
    this.productCategoryService.getAll().subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.productCategories = res.data;
      }
    });
  }

  fetchSpecification() {
    this.specificationService.getAll().subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.specifications = res.data;
      }
    });
  }

  fetchColor() {
    this.specificationCategoryService
      .getColorByCode('color')
      .subscribe((res) => {
        this.colors = res.data;
      });
  }

  override patchDataToForm(data: any) {
    super.patchDataToForm(data);

    // patch specification property in form
    let specificationId: number[] = [];
    if (this.isString(data?.specificationId)) {
      specificationId = (data?.specificationId.split(',') as string[]).map(
        (e) => +e
      );
    }
    this.drawerForm.get('specificationId')?.setValue(specificationId);

    // set action upload image
    this.uploadUrl = `https://localhost:7114/api/products/${data.id}/photos/true&${this}`;

    // set file list
    if (data.photos !== undefined) {
      this.fileList = (data.photos as Photo[])
        .filter((e) => e.isMain) // Filter the array to include only elements where IsMain is true
        .map((e) => ({
          uid: `${e.id}`,
          name: 'image.png',
          status: 'done',
          url: `${e.url}`,
          isMain: e.isMain,
        }));
    }
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
      price: [null, Validators.required],
      description: [null],
      productCategoryId: [null, Validators.required],
      specificationId: [null],
    });

    this.colorForm = this.fb.group({
      colorId: [null, Validators.required],
    });
  }

  override submitForm() {
    this.validateForm();
    if (this.drawerForm.valid) {
      this.isLoading = true;
      if (this.mode === 'create') {
        this.productService
          .create({
            ...this.drawerForm.getRawValue(),
            specificationId: this.drawerForm
              .get('specificationId')
              ?.value.join(','),
          })
          .pipe(finalize(() => (this.isLoading = false)))
          .subscribe((res) => {
            if (checkResponseStatus(res)) {
              this.message.success('Create successfully');
              this.changeToDetail();
              this.onCreate.emit(res.data);
            }
          });
      } else {
        this.productService
          .update(this.drawerForm.value.id, {
            ...this.drawerForm.getRawValue(),
            specificationId: this.drawerForm
              .get('specificationId')
              ?.value.join(','),
          })
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
    this.productService.delete(this.drawerForm.value.id).subscribe((res) => {
      if (checkResponseStatus(res)) {
        this.message.success('Delete successfully');
        this.closeDrawer();
        this.onDelete.emit(res.data);
      }
    });
  }

  // check is string
  isString(value: any): value is string {
    return typeof value === 'string';
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

      // Check require colorId when upload photo
      let colorId = this.colorForm?.get('colorId').value;
      if (colorId === undefined || colorId === null) {
        this.message.warning('Select a color');
        observer.complete();
      }

      observer.next(isJpgOrPng && isLt2M);
      observer.complete();
    });

  removePhoto = (file: NzUploadFile): boolean => {
    this.productService
      .removePhoto(this.data.id, +file.uid)
      .subscribe((res) => {
        if (checkResponseStatus(res)) {
          this.message.success('Remove successfully');
          this.changeToDetail();
        }
      });
    return true;
  };

  updateUploadUrl() {
    this.uploadUrl = `https://localhost:7114/api/products/${
      this.data?.id
    }/photos/true&${this.colorForm?.get('colorId').value}`;
    console.log(this.uploadUrl);
  }
}
