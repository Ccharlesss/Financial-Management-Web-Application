import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { AuthResponse } from '../models/auth.response.model';
import { Observable } from 'rxjs';
import { TokenService } from './token.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // --------------------------------------------------------------------------
  // Dependency injection:
  private httpClient = inject(HttpClient);
  private tokenService = inject(TokenService);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // List of Endpoints for forwardin queries to appropriate backend services:
  private readonly registerUrl = 'http://localhost:8088/api/account/register';
  private readonly signInUrl = 'http://localhost:8088/api/account/login';
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Handles the sign up process:
  handleSignUp(email: string, password: string) {
    const body = { email: email, password: password };
    // Send the sign up request to the backend:
    return this.httpClient.post(this.registerUrl, body);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Handles the sign in process:
  handleSignIn(email: string, password: string): Observable<AuthResponse> {
    const body = { email: email, password: password };
    return this.httpClient.post<AuthResponse>(this.signInUrl, body);
  }
  // --------------------------------------------------------------------------
}
