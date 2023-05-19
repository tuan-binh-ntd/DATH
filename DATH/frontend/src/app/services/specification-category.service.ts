import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SpecificationCategory } from '../models/specificationCategory.model';
import { PaginationResult } from '../models/pagination-result';
import { ResponseResult } from '../models/response';

@Injectable({
  providedIn: 'root'
})
export class SpecificationCategoryService {
  baseUrl: string = environment.baseUrl + 'specificationcategorys';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<SpecificationCategory>> {
    if (pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<SpecificationCategory>>(this.baseUrl);
    return this.http.get<ResponseResult<SpecificationCategory>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }

  get(id: number) {
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: SpecificationCategory): Observable<any> {
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: SpecificationCategory): Observable<any> {
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + '/' + id);
  }

  getColorByCode(code: string): Observable<ResponseResult<any>> {
    return this.http.get<ResponseResult<any>>(this.baseUrl + '/' + code + '/specifications');
  }
}
