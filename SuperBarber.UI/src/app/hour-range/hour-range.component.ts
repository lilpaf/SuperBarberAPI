import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HourValidatorService } from './hour-validator.service';
import { NgSelectModule } from '@ng-select/ng-select';
import { DayHoursDto } from '../models/day-hours-dto';

@Component({
  selector: 'app-hour-range',
  standalone: true,
  imports: [FormsModule, CommonModule, NgSelectModule],
  templateUrl: './hour-range.component.html',
  styleUrl: './hour-range.component.css',
})
export class HourRangeComponent implements OnInit {
  startHours: string[] = [];
  endHours: string[] = [];
  startTime?: string;
  endTime?: string;
  @Output() timeSelectedEvent = new EventEmitter<DayHoursDto>();

  constructor(private hourValidatorService: HourValidatorService) {}

  ngOnInit(): void {
    for (let i = 0; i <= 24; i++) {
      if (i <= 23) {
        this.addHourAndHalfHour(this.startHours, i);
      }
      if (i > 0) {
        this.addHourAndHalfHour(this.endHours, i);
      }
    }
  }

  private addHourAndHalfHour(hours: string[], hour: number) {
    const hourString = hour < 10 ? `0${hour}` : `${hour}`;
    hours.push(`${hourString}:00`);

    if (hour < 24) {
      hours.push(`${hourString}:30`);
    }
  }

  timeSelected() {
    this.hourValidatorService.validateRange(this.startTime, this.endTime);
    const value: DayHoursDto = {
      openingTime: this.startTime,
      closingTime: this.endTime,
    };
    this.timeSelectedEvent.emit(value);
  }
}
