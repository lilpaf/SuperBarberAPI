import { Component, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CityDto } from './models/city-dto';
import {
  HttpClient,
  HttpClientModule,
  HttpErrorResponse,
} from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ResponseContent } from '../models/response-content';
import { ErrorResponse } from '../models/error-response';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [FormsModule, HttpClientModule, CommonModule],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css',
})
export class HomePageComponent implements OnInit {
  cities: CityDto[] = [];
  selectedCity!: CityDto;
  errorMessage = '';

  constructor(private httpClient: HttpClient) {}

  ngOnInit(): void {
    //ToDo fix me into service
    this.httpClient
      .get<ResponseContent<CityDto[]>>('https://localhost:7193/cities/all')
      .subscribe({
        next: (response) => {
          this.cities = response.result;
          const sofia = this.cities.find((x) => x.name === 'София');

          if (sofia) {
            this.selectedCity = sofia;
          }

          console.log(this.cities);
        },
        error: (error: HttpErrorResponse) => {
          //ToDo fix me
          this.errorMessage = (error.error as ErrorResponse).errorMessage;
          console.log(this.errorMessage);
          console.log(error);
        },
      });
  }

  onSubmit(form: NgForm) {
    console.log(this.selectedCity);
  }
}
