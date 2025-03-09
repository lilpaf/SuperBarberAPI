import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpService } from '../../services/http.service';
import { LoginRequest } from '../models/login-request';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  email = '';
  password = '';
  rememberMe = false;

  constructor(private httpService: HttpService, private router: Router) {}

  onSubmit(form: NgForm) {
    // ToDo handle !!!
    if (form.invalid) {
      return;
    }
    const request: LoginRequest = {
      email: this.email,
      password: this.password,
    };

    this.httpService.login(request).subscribe({
      next: (response) => {
        this.router.navigate(['/home']);
      },
      error: () => {
        // ToDo handle !!!
      },
    });
  }
}
