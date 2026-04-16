import { Component, inject } from '@angular/core';
import { HeaderItem } from './header.model';
import { AuthService } from '../../../core/services/auth.service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {

  private authService = inject(AuthService);
  private router = inject(Router);
  protected email = this.authService.email;
  
  headers:HeaderItem[] = [
    {
      label: 'Home',
      route: '/home'
    },
    {
      label: 'About',
      route: '/about'
    },
  ]

  onLogout(){
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
