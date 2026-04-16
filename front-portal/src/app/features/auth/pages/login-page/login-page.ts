import { Component, effect, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {

  protected authService = inject(AuthService);
  private router = inject(Router);


  form = new FormGroup({
    email: new FormControl<string>('', {
      validators: [Validators.required, Validators.email, Validators.maxLength(250), Validators.minLength(3)],
      nonNullable: true,
    })
  });


  private readonly formChanges = toSignal(this.form.valueChanges);

  private readonly clearErrorEffect = effect(() => {
    this.formChanges();
    this.authService.clearError();
  });
  private readonly navigationEffect = effect(() => {
    console.log('Navigation effect triggered', this.authService.email());
  if (this.authService.email()) {
    this.form.reset();
    this.router.navigate(['/home']);
  }
});

  onSignIn(){
    this.authService.clearError();
    if(!this.form.value.email || !this.form.valid) return;
    this.authService.signIn(this.form.value.email);
    
  }

  onSignUp(){
    this.authService.clearError();
    if(!this.form.value.email || !this.form.valid) return;
    
    this.authService.signUp(this.form.value.email);
  }

  onInputChange(){
    this.form.markAsPristine();
    this.form.updateValueAndValidity();
    this.authService.clearError();
  }
}
