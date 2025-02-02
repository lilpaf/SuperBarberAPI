import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { CityDto } from '../models/city-dto';
import { ErrorResponse } from '../models/error-response';
import { ResponseContent } from '../models/response-content';
import { BarbershopsListRequest } from '../models/barbershops-list-request';
import { ListBarberShopsResponseDto } from '../models/list-barber-shops-response';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  private apiUrl: string;

  constructor(private httpClient: HttpClient) {
    this.apiUrl = environment.apiUrl;
  }

  getAllCitiesAndNeighborhoods() {
    return this.httpClient.get<ResponseContent<CityDto[]>>(
      `${this.apiUrl}/cities/all`
    );
  }

  getAllPublicBarberShops(request: BarbershopsListRequest) {
    const url = new URL(`${this.apiUrl}/barber-shop/all`);
    url.search = this.buildQueryParams(request);

    return this.httpClient.get<ResponseContent<ListBarberShopsResponseDto>>(
      url.toString()
    );
  }

  private buildQueryParams(queryStringParams: { [key: string]: any }) {
    return Object.entries(queryStringParams)
      .map(
        ([key, value]) =>
          `${encodeURIComponent(key)}=${encodeURIComponent(value || '')}`
      )
      .join('&');
  }
}
