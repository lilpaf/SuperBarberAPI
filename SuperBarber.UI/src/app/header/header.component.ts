import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  imagePath = 'assets/logo.png';
  showProfile: boolean = false;
  notificationsCount: number = 0;

  ngOnInit(): void {
    // if autenticated show profile image
    this.showProfile = false;
  }
}
