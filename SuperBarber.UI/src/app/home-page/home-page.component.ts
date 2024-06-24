import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CityDto } from './models/city-dto';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ResponseContent } from '../models/response-content';
import { ErrorResponse } from '../models/error-response';
import { NgSelectModule } from '@ng-select/ng-select';
import { NeighborhoodDto } from './models/neighborhood-dto';
import {
  NgbCalendar,
  NgbDate,
  NgbDatepickerModule,
  NgbDateStruct,
  NgbInputDatepicker,
} from '@ng-bootstrap/ng-bootstrap';
import { HourRangeComponent } from '../hour-range/hour-range.component';
import { HourValidatorService } from '../hour-range/hour-validator.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-home-page',
  standalone: true,
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css',
  imports: [
    FormsModule,
    CommonModule,
    NgSelectModule,
    NgbDatepickerModule,
    HourRangeComponent,
  ],
})
export class HomePageComponent implements OnInit, OnDestroy {
  cities: CityDto[] = [];
  selectedCity!: CityDto;
  selectedNeighborhood?: NeighborhoodDto | null;
  errorResponse!: ErrorResponse;
  cityLoading = false;
  neighborhoodLoading = false;
  today!: NgbDate;
  maxDate!: NgbDateStruct;
  date!: NgbDateStruct;
  timeIsValid = true;
  private timeSubscription!: Subscription;

  constructor(
    private httpClient: HttpClient,
    private calendar: NgbCalendar,
    private timeValidatorService: HourValidatorService
  ) {}

  ngOnInit(): void {
    this.today = this.calendar.getToday();
    this.maxDate = this.calendar.getNext(this.today, 'm', 2);
    this.date = this.today;

    this.timeSubscription =
      this.timeValidatorService.validationResult.subscribe((isValid) => {
        this.timeIsValid = isValid;
      });

    this.cityLoading = true;
    //ToDo fix me into service
    this.httpClient
      .get<ResponseContent<CityDto[]>>('https://localhost:7193/cities/all')
      .subscribe({
        next: (response) => {
          this.cities = response.result;
          const sofia = this.cities.find((x) => x.name === 'София');

          if (sofia) {
            this.selectedCity = sofia;
            this.getNeighborhood();
          }

          this.cityLoading = false;
        },
        error: (error: HttpErrorResponse) => {
          this.errorResponse = error.error.error as ErrorResponse;
          console.log(this.errorResponse);
          console.log(error);
          this.cityLoading = false;
        },
      });
  }

  getNeighborhood() {
    let neighborhood = null;

    if (this.selectedCity && this.selectedCity.neighborhoods) {
      neighborhood = this.selectedCity.neighborhoods.find(
        (x) => x.name === 'Център'
      );
    }

    this.selectedNeighborhood = neighborhood;
  }

  onCalendarButtonClicked(datePicker: NgbInputDatepicker) {
    datePicker.toggle();
  }

  onTodayClicked(datePicker: NgbInputDatepicker) {
    this.date = this.today;
    this.closeCalendar(datePicker);
  }

  closeCalendar(datePicker: NgbInputDatepicker) {
    datePicker.close();
  }

  onSubmit(form: NgForm) {
    console.log(form);
    console.log(this.selectedCity);
    console.log(this.selectedNeighborhood);
    console.log(this.date);
  }

  ngOnDestroy(): void {
    this.timeSubscription.unsubscribe();
  }
}
