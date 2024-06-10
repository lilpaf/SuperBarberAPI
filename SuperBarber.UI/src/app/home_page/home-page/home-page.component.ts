import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent {
  cities: string[] = [];
  neigbourhoods


  onSubmit(form: NgForm) {

  }
}
