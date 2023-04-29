import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';

@Injectable()
export class DefaultInterceptor implements HttpInterceptor {

  constructor(private cookieService: CookieService,
     private router: Router,
     private msg: NzMessageService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request.clone({
      setHeaders: {
        Authorization: `Bearer ` + this.cookieService.get('token')
      }
    })).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMsg = '';
        if(error.status === 403){
          this.router.navigateByUrl('exception/403');
        }
        if(error.status === 401){
          this.msg.error("You must login first");
          this.router.navigateByUrl('passport/login');
        }
        return throwError(errorMsg);
      })
    );
  }
}
