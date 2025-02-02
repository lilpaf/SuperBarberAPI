import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import { ErrorResponse } from '../../models/error-response';
import { ErrorService } from '../error.service';

@Injectable({
  providedIn: 'root',
})
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private errorService: ErrorService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error) => {
        const errorResponse = error.error?.error as ErrorResponse;

        if (errorResponse) {
          this.errorService.sendError(errorResponse);
        }

        console.error(error);
        throw error;
      })
    );
  }
}
