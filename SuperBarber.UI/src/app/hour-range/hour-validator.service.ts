import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HourValidatorService {
  validationResult = new BehaviorSubject<boolean>(true);

  validateRange(startTime: string, endTime: string) {
    if (startTime && endTime) {
      const startHour = this.parseHour(startTime);
      const startMin = this.parseMinutes(startTime);

      const endHour = this.parseHour(endTime);
      const endMin = this.parseMinutes(endTime);

      if (startHour === endHour) {
        this.validationResult.next(startMin <= endMin);
        return;
      }

      this.validationResult.next(startHour < endHour);
      return;
    } else if (startTime || endTime) {
      this.validationResult.next(false);
      return;
    }

    this.validationResult.next(true);
  }

  private parseHour(hour: string) {
    return parseInt(hour.split(':')[0]);
  }

  private parseMinutes(hour: string) {
    return parseInt(hour.split(':')[1]);
  }
}
