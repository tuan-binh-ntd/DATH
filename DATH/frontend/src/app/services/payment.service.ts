import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ResponseResult } from '../models/response';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Payment } from '../models/payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  baseUrl: string = environment.baseUrl + 'payments';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<Payment>> {
    if (pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<Payment>>(this.baseUrl);
    return this.http.get<ResponseResult<Payment>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }

  get(id: number) {
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Payment): Observable<any> {
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Payment): Observable<any> {
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
