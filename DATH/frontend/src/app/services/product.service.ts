import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SpecificationCategory } from '../models/specificationCategory.model';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl: string = environment.baseUrl + 'products';

  constructor(private http: HttpClient) { }

  getAll(): Observable<any>{
    return this.http.get(this.baseUrl);
  }

  get(id: number){
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Product): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Product): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
