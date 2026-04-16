import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginPage } from './login-page';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { signal } from '@angular/core';
import { vi } from 'vitest';
import { provideRouter } from '@angular/router';

describe('LoginPage', () => {
  let fixture: ComponentFixture<LoginPage>;
  let authService: {
    signIn: ReturnType<typeof vi.fn>;
    signUp: ReturnType<typeof vi.fn>;
    clearError: ReturnType<typeof vi.fn>;
    email: ReturnType<typeof signal<string | null>>;
    error: ReturnType<typeof signal<string | null>>;
  };

  beforeEach(async () => {
    authService = {
      signIn: vi.fn(),
      signUp: vi.fn(),
      clearError: vi.fn(),
      email: signal(null),
      error: signal(null)
    };

    await TestBed.configureTestingModule({
      imports: [LoginPage],
      providers: [
        { provide: AuthService, useValue: authService },
        provideRouter([])
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginPage);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should disable sign in button when form is invalid', () => {
    const button = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBe(true);
  });

  it('should enable sign in button when form is valid', () => {
    const input = fixture.nativeElement.querySelector('input');
    input.value = 'test@test.com';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBe(false);
  });

  it('should call signIn with email when form is submitted', () => {
    const input = fixture.nativeElement.querySelector('input');
    input.value = 'test@test.com';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const form = fixture.nativeElement.querySelector('form');
    form.dispatchEvent(new Event('submit'));

    expect(authService.signIn).toHaveBeenCalledWith('test@test.com');
  });

  it('should call signUp with email when sign up button is clicked', () => {
    const input = fixture.nativeElement.querySelector('input');
    input.value = 'test@test.com';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const signUpButton = fixture.nativeElement.querySelectorAll('button')[1];
    signUpButton.click();

    expect(authService.signUp).toHaveBeenCalledWith('test@test.com');
  });

  it('should show validation error when email format is invalid', () => {
    const component = fixture.componentInstance as any;
    
    component.form.controls.email.setValue('notanemail');
    component.form.controls.email.markAsTouched();
    
    fixture.detectChanges();

    const errorP = fixture.nativeElement.querySelector('p.error');
    expect(errorP).toBeTruthy();
    expect(errorP.textContent).toContain('Invalid email');
  });
});