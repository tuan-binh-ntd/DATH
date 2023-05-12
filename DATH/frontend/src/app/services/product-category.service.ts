import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ProductCategory } from '../models/product-category.model';
import { ResponseResult } from '../models/response';
import { Specification } from '../models/specification.model';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductCategoryService {
  baseUrl: string = environment.baseUrl + 'productcategorys/';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<ProductCategory>> {
    if (pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<ProductCategory>>(this.baseUrl);
    else return this.http.get<ResponseResult<ProductCategory>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }

  getAllBySpecificationById(id: number){
    return this.http.get<ResponseResult<Specification>>(this.baseUrl + id + '/specifications');
  }

  get(id: number){
    return this.http.get(this.baseUrl + id);
  }

  update(id: number, payload: ProductCategory): Observable<any>{
    return this.http.put(this.baseUrl + id, payload);
  }

  create(payload: ProductCategory): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + id);
  }

  getAllByCategory(
    id: number,
    pageNum?: number,
    pageSize?: number,
    specificationIds?: string,
    keyword?: string,
  ): Observable<ResponseResult<Product>> {
    if (pageNum === undefined && pageSize === undefined)
      return this.http.get<ResponseResult<Product>>(this.baseUrl);
      var keywordString = ""
      if(keyword) keywordString +=  '&keyword=' + keyword
    return this.http.get<ResponseResult<Product>>(
      this.baseUrl +
        id +
        '/products' +
        '?pageNum=' +
        pageNum +
        '&pageSize=' +
        pageSize +
        '&specificationIds=' +
        specificationIds +
        keywordString
    );
  }
}
