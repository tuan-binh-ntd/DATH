import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Installment } from '../models/installment.model';
import { ResponseResult } from '../models/response';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class InstallmentService {
  baseUrl: string = environment.baseUrl + 'installments';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<Installment>> {
    if (pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<Installment>>(this.baseUrl);
    else return this.http.get<ResponseResult<Installment>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }

  get(id: number){
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Installment): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Installment): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
