import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../services/api.AuthService';
import { TokenService } from '../../services/token.service';
import { AuthResponse } from '../../models/auth.response.model';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  // ----------------------------------------------------------------
  // Signal properties to track:
  isAuthenticated = signal(false);
  // ----------------------------------------------------------------
  // Dependencies:
  private authService = inject(AuthService);
  private tokenService = inject(TokenService);
  private router = inject(Router);
  // ----------------------------------------------------------------

  // ----------------------------------------------------------------
  // Create a FormGrp to get user's credentials required to authenticate:
  form = new FormGroup({
    email: new FormControl('', {
      validators: [Validators.email, Validators.required],
    }),
    password: new FormControl('', {
      validators: [Validators.required, Validators.minLength(9)],
    }),
  });

  // ----------------------------------------------------------------
  // Handles the submission of the sign in form:
  onSubmit() {
    if (this.form.invalid) {
      return;
    }
    const { email, password } = this.form.value;
    this.authService.handleSignIn(email!, password!).subscribe({
      next: (response: AuthResponse) => {
        console.log('User successfully signed in.', response);
        const token = response?.token;
        if (token) {
          this.tokenService.setCookie(token);
          this.isAuthenticated.set(true);
          this.form.reset();
          this.router.navigate(['dashboard']);
        }
      },

      error: (err) => {
        console.log('Authentication failed', err);
        alert(err?.error || 'Authentication failed');
      },
    });
  }
  // ----------------------------------------------------------------
}
