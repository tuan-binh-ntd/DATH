import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';
import { Register } from '../models/register.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string = environment.baseUrl + 'account/';
  currentUserSource = new BehaviorSubject<Account>({
    name: null,
    token: null,
  });

  // currentUser = this.currentUserSource.asObservable();
  constructor(private http: HttpClient) { }

  signIn(payload: Account): Observable<any> {
    return this.http.post<any>(this.baseUrl + 'login', payload).pipe(
      map((response: any) => {
        const user = response;
        if(user) {
          this.setCurrentUser(user.data);
        }
        return user;
      })
    );
  }

  setCurrentUser(account: Account) {
    localStorage.setItem('user', JSON.stringify(account));
    this.currentUserSource.next(account);
  }

  signUp(payload: Register): Observable<Account> {
    return this.http.post<Account>(this.baseUrl + 'customer', payload);
  }

  register(payload: Register): Observable<Account> {
    return this.http.post<Account>(this.baseUrl + 'employee', payload);
  }
}
