import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Feedback } from '../models/feedback.model';
import { ResponseResult } from '../models/response';


@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  baseUrl: string = environment.baseUrl + 'feedbacks';

  constructor(private http: HttpClient) { }

  getAll(pageNum?: number, pageSize?: number): Observable<ResponseResult<Feedback>> {
    if (pageNum === undefined && pageSize === undefined) return this.http.get<ResponseResult<Feedback>>(this.baseUrl);
    else return this.http.get<ResponseResult<Feedback>>(this.baseUrl + '?pageNum=' + pageNum + '&pageSize=' + pageSize);
  }

  update(id: number, payload: Feedback): Observable<any>{
    return this.http.put(this.baseUrl + '/' + id, payload);
  }

  create(payload: Feedback): Observable<any>{
    return this.http.post(this.baseUrl, payload);
  }


}
