import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account.model';
import { Register } from '../models/register.model';
import { Login } from '../models/login.model';
import { CookieService } from 'ngx-cookie-service';
import { ChangePassword } from '../models/change-password.model';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl: string = environment.baseUrl + 'accounts/';
  currentUserSource = new ReplaySubject<Account | null>(1);

  // currentUser = this.currentUserSource.asObservable();
  constructor(private http: HttpClient, private cookieService: CookieService) {}

  signIn(payload: Login): Observable<any> {
    return this.http.post<any>(this.baseUrl + 'login', payload).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user.data);
          this.cookieService.set(
            'token',
            user.data.token,
            7,
            '/',
            undefined,
            true,
            'None'
          );
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
    return this.http.post<Account>(this.baseUrl + 'customers', payload).pipe(
      map((response: any) => {
        debugger
        const user = response;
        if (user) {
          this.setCurrentUser(user.data);
        }
        return user;
      })
    );
  }

  logOut(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  register(payload: Register): Observable<any> {
    return this.http.post<Account>(this.baseUrl + 'employees', payload).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user.data);
        }
        return user;
      })
    )
  }

  checkUsername(username: string): Observable<any> {
    return this.http.get<Account>(this.baseUrl + username);
  }

  changePassword(username: string, payload: ChangePassword): Observable<any> {
    return this.http.put(this.baseUrl + username, payload);
  }
}
