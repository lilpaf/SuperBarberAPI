import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CityDto } from '../models/city-dto';
import { CommonModule } from '@angular/common';
import { NgSelectModule } from '@ng-select/ng-select';
import { NeighborhoodDto } from '../models/neighborhood-dto';
import {
  NgbCalendar,
  NgbDate,
  NgbDatepickerModule,
  NgbDateStruct,
  NgbInputDatepicker,
} from '@ng-bootstrap/ng-bootstrap';
import { HttpService } from '../services/http.service';
import { Router } from '@angular/router';
import { BarbershopsListRequest } from '../models/barbershops-list-request';

@Component({
  selector: 'app-home-page',
  standalone: true,
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css',
  imports: [FormsModule, CommonModule, NgSelectModule, NgbDatepickerModule],
})
export class HomePageComponent implements OnInit {
  cities: CityDto[] = [];
  selectedCity!: CityDto;
  selectedNeighborhood?: NeighborhoodDto | null;
  cityLoading = false;
  neighborhoodLoading = false;
  today!: NgbDate;
  maxDate!: NgbDateStruct;
  date!: NgbDateStruct;

  constructor(
    private httpService: HttpService,
    private calendar: NgbCalendar,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.today = this.calendar.getToday();
    this.maxDate = this.calendar.getNext(this.today, 'm', 2);
    this.date = this.today;

    this.cityLoading = true;

    this.httpService.getAllCitiesAndNeighborhoods().subscribe({
      next: (response) => {
        this.cities = response.result;
        const sofia = this.cities.find((x) => x.name === 'София');

        if (sofia) {
          this.selectedCity = sofia;
          this.getNeighborhood();
        }

        this.cityLoading = false;
      },
      error: () => {
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
    const request: BarbershopsListRequest = {
      city: this.selectedCity.name,
      neighborhood: this.selectedNeighborhood?.name,
      date: this.formatNgbDateToString(this.date),
      currentPage: 1,
    };

    this.router.navigate(['barber-shops'], {
      queryParams: { ...request },
    });
  }

  formatNgbDateToString(date: NgbDateStruct): string {
    const year = date.year;
    const month = date.month.toString().padStart(2, '0');
    const day = date.day.toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
