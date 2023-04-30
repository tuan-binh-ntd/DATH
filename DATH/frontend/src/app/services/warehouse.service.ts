import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ResponseResult } from '../models/response';
import { Warehouse } from '../models/warehouse.model';

@Injectable({
  providedIn: 'root'
})
export class WarehouseService {
  baseUrl: string = environment.baseUrl + 'warehouses';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<Warehouse>> {
    if (pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<Warehouse>>(this.baseUrl);
    else return this.http.get<ResponseResult<Warehouse>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }

  get(id: number){
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Warehouse): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Warehouse): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
