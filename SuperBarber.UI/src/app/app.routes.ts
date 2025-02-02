import { Routes } from '@angular/router';
import { HomePageComponent } from './home-page/home-page.component';
import { BarbershopsListComponent } from './barbershops-list/barbershops-list.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomePageComponent },
  {
    path: 'barber-shops',
    component: BarbershopsListComponent,
  },
];
