import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    sessionStorage.clear();
    
    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });


  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return false when no token exists', () => {
    expect(service.isAuthenticated()).toBe(false);
  });

  it('should authenticate user after successful signIn', () => {
    service.signIn('test@test.com');

    const req = httpMock.expectOne(`${environment.userApiUrl}/user/login`);
    expect(req.request.method).toBe('POST');
    
    req.flush({
      email: 'test@test.com',
      accessToken: 'fake-access-token',
      refreshToken: 'fake-refresh-token',
      expiresIn: 3600,
      tokenType: 'Bearer'
    });

    expect(service.isAuthenticated()).toBe(true);
    expect(service.email()).toBe('test@test.com');
  });

  it('should clear state after logout', () => {
    service.signIn('test@test.com');
    const req = httpMock.expectOne(`${environment.userApiUrl}/user/login`);
    req.flush({
      email: 'test@test.com',
      accessToken: 'fake-access-token',
      refreshToken: 'fake-refresh-token',
      expiresIn: 3600,
      tokenType: 'Bearer'
    });

    service.logout();

    expect(service.isAuthenticated()).toBe(false);
    expect(service.email()).toBeNull();
    expect(sessionStorage.getItem('accessToken')).toBeNull();
  });

  it('should not authenticate user after failed signIn', () => {
    service.signIn('wrong@test.com');

    const req = httpMock.expectOne(`${environment.userApiUrl}/user/login`);
    
    req.flush('User not found', { 
      status: 404, 
      statusText: 'Not Found' 
    });

    expect(service.isAuthenticated()).toBe(false);
  });

  it('should restore session from sessionStorage', () => {
    sessionStorage.setItem('accessToken', 'stored-token');
    sessionStorage.setItem('email', 'stored@test.com');

    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [AuthService, provideHttpClient(), provideHttpClientTesting()]
    });
    const freshService = TestBed.inject(AuthService);

    expect(freshService.isAuthenticated()).toBe(true);
    expect(freshService.email()).toBe('stored@test.com');
  });
});