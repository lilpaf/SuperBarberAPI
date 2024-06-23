import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CityDto } from './models/city-dto';
import {
  HttpClient,
  HttpClientModule,
  HttpErrorResponse,
} from '@angular/common/http';
import { CommonModule, DatePipe, formatDate } from '@angular/common';
import { ResponseContent } from '../models/response-content';
import { ErrorResponse } from '../models/error-response';
import { NgSelectModule } from '@ng-select/ng-select';
import { NeighborhoodDto } from './models/neighborhood-dto';
import {
  NgbCalendar,
  NgbDatepickerModule,
  NgbDateStruct,
} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [
    FormsModule,
    HttpClientModule,
    CommonModule,
    NgSelectModule,
    NgbDatepickerModule,
  ],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css',
})
export class HomePageComponent implements OnInit {
  cities: CityDto[] = [];
  selectedCity!: CityDto;
  selectedNeighborhood?: NeighborhoodDto | null;
  errorResponse!: ErrorResponse;
  cityLoading = false;
  neighborhoodLoading = false;
  today = inject(NgbCalendar).getToday();
  model: NgbDateStruct;

  constructor(private httpClient: HttpClient) {}

  ngOnInit(): void {
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
          console.log(this.cities);
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
    //ToDo undefined maybe?
    let neighborhood = null;

    if (this.selectedCity.neighborhoods) {
      neighborhood = this.selectedCity.neighborhoods.find(
        (x) => x.name === 'Център'
      );
    }

    this.selectedNeighborhood = neighborhood;
  }

  onSubmit(form: NgForm) {
    console.log(form);
    console.log(this.selectedCity);
    console.log(this.selectedNeighborhood);
  }
}
