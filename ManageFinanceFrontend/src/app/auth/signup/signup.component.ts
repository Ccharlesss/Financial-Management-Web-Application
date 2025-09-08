import { Component, ViewChild, inject, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { AuthService } from '../../services/api.AuthService';
import { ToastComponent } from '../../toast/toast/toast.component';

@Component({
  selector: 'app-signup',
  imports: [ReactiveFormsModule, ToastComponent],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css',
})
export class SignupComponent {
  // ----------------------------------------------------------------
  isSignedUp = signal(false);
  // ----------------------------------------------------------------
  @ViewChild(ToastComponent) toast!: ToastComponent;
  // ----------------------------------------------------------------
  // Dependency injection to use function defined in AuthService:
  private authService = inject(AuthService);
  // ----------------------------------------------------------------

  // ----------------------------------------------------------------
  // Create a new FormGrp to hold the inputs entered in the form to signup:
  form = new FormGroup({
    email: new FormControl('', {
      validators: [Validators.email, Validators.required],
    }),
    password: new FormControl('', {
      validators: [Validators.required, Validators.minLength(9)],
    }),
  });

  // ----------------------------------------------------------------
  // Handles the form Submission:
  onSubmit() {
    if (this.form.invalid) {
      this.toast.showToast(
        '❌ Registration failed. Please try again.',
        'error'
      );
      return;
    }

    const { email, password } = this.form.value;
    console.log(email);
    console.log(password);

    this.authService.handleSignUp(email!, password!).subscribe({
      next: (response) => {
        console.log('User registered:', response);
        this.isSignedUp.set(true);
        this.toast.showToast(
          '✅ Signup successful! an activation link was sent to your email address',
          'success'
        );
        this.form.reset();
      },

      error: (err) => {
        console.error('Registration failed:', err);
        alert(err?.error || 'registration failed.');
      },
    });
  }
}
