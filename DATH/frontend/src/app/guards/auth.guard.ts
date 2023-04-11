import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService,
    private router: Router){}
  canActivate(): Observable<boolean> {
    return this.accountService.currentUserSource.pipe(
      map(user => {
        if(user?.token) {
          return true;
        }
        else {
          this.router.navigateByUrl('passport/login');
          return false;
        }
      })
    );
  }
  
}
