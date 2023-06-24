import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { map, Observable, of } from 'rxjs';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class CustomerAuthGuard implements CanActivate {
  constructor(private accountService: AccountService,
    private router: Router){}
    canActivate(): Observable<boolean> {
      if (this.accountService.currentUserSource.closed || this.accountService.currentUserSource.observers.length === 0) {
        this.router.navigateByUrl('/home');
        return of(false);
      }
  
      return this.accountService.currentUserSource.pipe(
        map(user => {
          if (user === null) {
            this.router.navigateByUrl('/home');
            return false;
          }
          return true;
        })
      );
    }
}
