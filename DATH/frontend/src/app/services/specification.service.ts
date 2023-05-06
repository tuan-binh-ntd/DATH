import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Specification } from '../models/specification.model';
import { ResponseResult } from '../models/response';

@Injectable({
  providedIn: 'root'
})
export class SpecificationService {
  baseUrl: string = environment.baseUrl + 'specifications';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<Specification>> {
    if(pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<Specification>>(this.baseUrl);
    return this.http.get<ResponseResult<Specification>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }



  get(id: number){
    return this.http.get(this.baseUrl + '/' + id);
  }

  update(id: number, payload: Specification): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Specification): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
