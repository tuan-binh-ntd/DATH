import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';
import { Register } from '../models/register.model';
import { Login } from '../models/login.model';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string = environment.baseUrl + 'account/';
  currentUserSource = new BehaviorSubject<Account>({
    username: null,
    firstName: null,
    lastName: null,
    gender: null,
    address: null,
    idNumber: null,
    phone: null,
    birthday: null,
    email: null,
    isActive: null,
    token: null,
    avatarUrl: null,
  });

  // currentUser = this.currentUserSource.asObservable();
  constructor(private http: HttpClient) { }

  signIn(payload: Login): Observable<any> {
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

  signUp(payload: Register): Observable<any> {
    return this.http.post<Account>(this.baseUrl + 'customers', payload);
  }

  register(payload: Register): Observable<any> {
    return this.http.post<Account>(this.baseUrl + 'employees', payload);
  }
}
