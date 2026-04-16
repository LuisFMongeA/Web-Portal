import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AuthResponse } from '../../features/auth/models/auth.model';
import { ERROR_MESSAGES } from '../constants/error-messages';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private httpClient = inject(HttpClient);

  private readonly loginUrl = `${environment.userApiUrl}/user/login`;
  private readonly registerUrl = `${environment.userApiUrl}/user/create`;
  private readonly refreshUrl = `${environment.authApiUrl}/auth/refresh`;
  
  private _accessToken = signal<string | null>(sessionStorage.getItem('accessToken'));
  private _email = signal<string | null>(sessionStorage.getItem('email'));
  private _refreshToken = signal<string | null>(sessionStorage.getItem('refreshToken'));
  private _error = signal<string | null>(null);

  readonly accessToken = this._accessToken.asReadonly();
  readonly refreshToken = this._refreshToken.asReadonly();
  readonly email = this._email.asReadonly();
  readonly error = this._error.asReadonly();

  



  signUp(email:string){
    this.httpClient.post<AuthResponse>(this.registerUrl, { email })
    .subscribe({
      next:() => {
        window.alert('Registration successful! Please log in with your new credentials.');
      },
      error: (err) => {
        switch(err.status) {
          case 400:
            this._error.set(ERROR_MESSAGES.auth.invalidEmail);
            break;
          case 409:
            this._error.set(ERROR_MESSAGES.auth.emailAlreadyRegistered);
            break;
          default:
            this._error.set(ERROR_MESSAGES.auth.unknown);
        }
      }
  });
  }

  signIn(email: string) {
    this.httpClient.post<AuthResponse>(this.loginUrl,  { email })
        .subscribe({
          next: (response) => {
            this._email.set(response.email);
            this._accessToken.set(response.accessToken);
            this._refreshToken.set(response.refreshToken);
            sessionStorage.setItem('accessToken', response.accessToken);
            sessionStorage.setItem('email', response.email);
            sessionStorage.setItem('refreshToken', response.refreshToken);
          },
          error: (err) => {
            this._email.set(null)
              switch(err.status) {
                case 400:
                  this._error.set(ERROR_MESSAGES.auth.invalidEmail);
                  break;
                  case 404:
                  this._error.set(ERROR_MESSAGES.auth.userNotFound);
                  break;
                case 409:
                  this._error.set(ERROR_MESSAGES.auth.emailAlreadyRegistered);
                  break;
                default:
                  this._error.set(ERROR_MESSAGES.auth.unknown);
              }
            }
        });
  }

  refresh() {
    return this.httpClient.post<AuthResponse>(this.refreshUrl, { 
      refreshToken: this._refreshToken() 
    }).pipe(
      tap((response) => {
        this._accessToken.set(response.accessToken);
        this._refreshToken.set(response.refreshToken);
        sessionStorage.setItem('accessToken', response.accessToken);
        sessionStorage.setItem('refreshToken', response.refreshToken);
      })
    );
  } 
  
 logout(): void {
  this._email.set(null);
  this._accessToken.set(null);
  this._refreshToken.set(null);
  sessionStorage.clear();
}

  isAuthenticated(): boolean {
    return this._accessToken() !== null;
  }

  clearError(): void {
  this._error.set(null);
  }
  

}
