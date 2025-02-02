import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ErrorResponse } from '../models/error-response';
import { ErrorService } from '../services/error.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-popup',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './error-popup.component.html',
  styleUrl: './error-popup.component.css',
})
export class ErrorPopupComponent implements OnInit, OnDestroy {
  errorMessages: string[] = [];
  private errorSubscription!: Subscription;

  constructor(private errorService: ErrorService) {}

  ngOnInit(): void {
    this.errorSubscription = this.errorService.error$.subscribe((error) => {
      if (error) {
        this.errorMessages.push(error.errorMessage);
        error.errorsMessages?.forEach((x) => this.errorMessages.push(x));
      } else {
        this.errorMessages = [];
      }
    });
  }

  ngOnDestroy(): void {
    if (this.errorSubscription) {
      this.errorSubscription.unsubscribe();
    }
  }

  closeError(): void {
    this.errorService.clearError();
  }
}
