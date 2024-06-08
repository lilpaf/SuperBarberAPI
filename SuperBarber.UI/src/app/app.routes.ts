import { Routes } from '@angular/router';
import { HomePageComponent } from './home_page/home-page/home-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomePageComponent },
];
