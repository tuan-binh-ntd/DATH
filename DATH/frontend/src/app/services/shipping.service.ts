import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Shipping } from '../models/shipping.model';

@Injectable({
  providedIn: 'root'
})
export class ShippingService {
  baseUrl: string = environment.baseUrl + 'shippings';

  constructor(private http: HttpClient) { }

  getAll(): Observable<any>{
    return this.http.get(this.baseUrl);
  }

  get(id: number){
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Shipping): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Shipping): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
