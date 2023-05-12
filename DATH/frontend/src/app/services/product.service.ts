import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Product } from '../models/product.model';
import { ResponseResult } from '../models/response';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseUrl: string = environment.baseUrl + 'products';

  constructor(private http: HttpClient) {}

  getAll(
    pageNum?: number,
    pageSize?: number
  ): Observable<ResponseResult<Product>> {
    if (pageNum === undefined && pageSize === undefined)
      return this.http.get<ResponseResult<Product>>(this.baseUrl);
    return this.http.get<ResponseResult<Product>>(
      this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize
    );
  }

  getAllByCategory(
    categoryId: number,
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
        '/by-category/' +
        categoryId +
        '?pageNum=' +
        pageNum +
        '&pageSize=' +
        pageSize +
        '&SpecificationIds=' +
        "38" +
        keywordString
    );
  }

  get(id: number) {
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Product): Observable<any> {
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Product): Observable<any> {
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + '/' + id);
  }

  removePhoto(id: number, photoId: number): Observable<any> {
    return this.http.delete(
      this.baseUrl + '/' + id + '/photos' + '/' + photoId
    );
  }
}
