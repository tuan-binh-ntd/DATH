import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string = environment.baseUrl + 'account';
  constructor(private http: HttpClient) { }

  signIn(payload: Account): Observable<Account> {
    return this.http.post<Account>(this.baseUrl + 'login', payload);
  }

  signUp(payload: Account): Observable<Account> {
    return this.http.post<Account>(this.baseUrl, payload);
  }
}
