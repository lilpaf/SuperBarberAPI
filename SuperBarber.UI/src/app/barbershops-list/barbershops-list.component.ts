import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { BarbershopsListRequest } from '../models/barbershops-list-request';
import { HttpService } from '../services/http.service';
import { ListBarberShopDto } from '../models/list-barber-shop-dto';

@Component({
  selector: 'app-barbershops-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './barbershops-list.component.html',
  styleUrl: './barbershops-list.component.css',
})
export class BarbershopsListComponent implements OnInit {
  constructor(
    private activatedRoute: ActivatedRoute,
    private httpService: HttpService
  ) {}

  barberShops: ListBarberShopDto[] = [];

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      const request: BarbershopsListRequest = {
        city: params['city'],
        neighborhood: params['neighborhood'],
        barberShopName: params['BarberShopName'],
        date: params['date'],
        currentPage: +params['currentPage'],
      };

      this.httpService.getAllPublicBarberShops(request).subscribe({
        next: (response) => {
          console.log(response);
          this.barberShops = response.result.barberShops;
        },
      });
    });
  }

  // getWorkingHours(barberShop: ListBarberShopDto) {
  //   return Object.keys(barberShop.workingWeekHours).map((day) => ({
  //     day,
  //     openingTime: barberShop.workingWeekHours[day].openingTime,
  //     closingTime: barberShop.workingWeekHours[day].closingTime,
  //   }));
  // }

  getRatingStars(barberShop: ListBarberShopDto) {
    const fullStars = Math.floor(barberShop.averageRating);
    const hasHalfStar = barberShop.averageRating % 1 >= 0.5;

    return Array(5)
      .fill(null)
      .map((_, index) => ({
        fill: index < fullStars,
        half: index === fullStars && hasHalfStar,
      }));
  }

  //ToDo fix it
  getImageUrl(barberShopId: number): string {
    return '';
  }
}
