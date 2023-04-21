import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Promotion } from '../models/promotion.model';
import { PaginationResult } from '../models/pagination-result';
import { ResponseResult } from '../models/response';

@Injectable({
  providedIn: 'root'
})
export class PromotionService {
  baseUrl: string = environment.baseUrl + 'promotions';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<Promotion>> {
    if(pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<Promotion>>(this.baseUrl);
    return this.http.get<ResponseResult<Promotion>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }

  get(id: number){
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Promotion): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Promotion): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
