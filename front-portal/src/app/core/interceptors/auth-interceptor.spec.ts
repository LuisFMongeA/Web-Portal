import { TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptors, HttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { authInterceptor } from './auth-interceptor';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { signal } from '@angular/core';
import { vi } from 'vitest';
import { throwError } from 'rxjs';

describe('authInterceptor', () => {
  let httpMock: HttpTestingController;
  let httpClient: HttpClient;
  let authService: { accessToken: ReturnType<typeof signal<string | null>>; logout: ReturnType<typeof vi.fn>; refresh: ReturnType<typeof vi.fn> };
  let router: { navigate: ReturnType<typeof vi.fn> };

  beforeEach(() => {
   authService = {
  accessToken: signal('fake-token'),
  logout: vi.fn(),
  refresh: vi.fn().mockReturnValue(throwError(() => new Error('refresh failed')))
};
    router = { navigate: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router }
      ]
    });

    httpMock = TestBed.inject(HttpTestingController);
    httpClient = TestBed.inject(HttpClient);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should add Authorization header when token exists', () => {
    httpClient.get('/test').subscribe();

    const req = httpMock.expectOne('/test');
    expect(req.request.headers.get('Authorization')).toBe('Bearer fake-token');
    req.flush({});
  });

  it('should not add Authorization header when no token', () => {
    authService.accessToken = signal(null);

    httpClient.get('/test').subscribe();

    const req = httpMock.expectOne('/test');
    expect(req.request.headers.get('Authorization')).toBeNull();
    req.flush({});
  });

  it('should call logout and navigate to login on 401', async () => {
    httpClient.get('/test').subscribe({ error: () => {} });

    const req = httpMock.expectOne('/test');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    await new Promise(resolve => setTimeout(resolve, 0));

    expect(authService.logout).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});