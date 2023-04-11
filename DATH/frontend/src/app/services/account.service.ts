import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';

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

  signIn(payload: Account): Observable<Account> {
    return this.http.post<Account>(this.baseUrl + 'login', payload).pipe(
      map((response: Account) => {
        const user = response;
        if(user) {  
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  setCurrentUser(account: Account) {
    localStorage.setItem('account', JSON.stringify(account));
    this.currentUserSource.next(account);
  }

  signUp(payload: Account): Observable<Account> {
    return this.http.post<Account>(this.baseUrl, payload);
  }
}
