import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ErrorResponse } from '../models/error-response';

@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  private errorSubject = new BehaviorSubject<ErrorResponse | null>(null);

  error$ = this.errorSubject.asObservable();

  sendError(error: ErrorResponse): void {
    this.errorSubject.next(error);
  }

  clearError(): void {
    this.errorSubject.next(null);
  }
}
