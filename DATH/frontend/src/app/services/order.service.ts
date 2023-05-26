import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Order } from '../models/order-model';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl: string = environment.baseUrl + 'orders';

  constructor(private http: HttpClient) { }

  create(payload: Order): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

}
