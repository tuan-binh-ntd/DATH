import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Employee } from '../models/employee.model';
import { ResponseResult } from '../models/response';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  baseUrl: string = environment.baseUrl + 'employees';

  constructor(private http: HttpClient) { }

  getAllEmployee(pageNum?: number, pageSize?: number): Observable<ResponseResult<Employee>> {
    if (pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<Employee>>(this.baseUrl);
    return this.http.get<ResponseResult<Employee>>(this.baseUrl + '/' + pageNum + '/' + pageSize);
  }

  update(id: number, payload: Employee): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  delete(id: number): Observable<any>{
    return this.http.delete(this.baseUrl + '/' + id);
  }
}
