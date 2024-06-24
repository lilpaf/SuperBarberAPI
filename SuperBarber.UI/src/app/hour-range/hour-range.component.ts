import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HourValidatorService } from './hour-validator.service';
import { NgSelectModule } from '@ng-select/ng-select';

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
  startTime!: string;
  endTime!: string;

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

  validateTime() {
    this.hourValidatorService.validateRange(this.startTime, this.endTime);
  }
}
