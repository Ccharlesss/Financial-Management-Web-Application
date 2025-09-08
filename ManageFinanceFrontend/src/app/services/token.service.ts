import { Injectable, inject } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  // --------------------------------------------------------------------------
  private readonly Token_KEY = 'auth_token';
  // --------------------------------------------------------------------------
  private cookieService = inject(CookieService);
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  // Store the token in a cookie:
  setCookie(token: string, expireHours: number = 1): void {
    // Create a new Date obj representing the current date and time => used to calculate when the cookie should expire
    const expires = new Date();
    // Set the time when the cookie expires: current time + 1h:
    expires.setHours(expires.getHours() + expireHours);
    // Creates a cookie: set the cookie name + values:
    this.cookieService.set(this.Token_KEY, token, {
      expires, // Expiration => after expiration, browser will automatically remove the token
      secure: true, // Ensures the cookie is only used in HTTPS request
      sameSite: 'Lax', // Controls whether the browser sends the cookie w/ cross-site requests => cookie send by clicking a link
      path: '/', // Ensures cookie is accessible from all routes of your app
    });
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  getCookie(): string {
    return this.cookieService.get(this.Token_KEY);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  deleteCookie(): void {
    this.cookieService.delete(this.Token_KEY, '/');
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  hasCookie(): boolean {
    return this.cookieService.check(this.Token_KEY);
  }
  // --------------------------------------------------------------------------

  // --------------------------------------------------------------------------
  getUserIdFromToken(): string | null {
    const token = this.getCookie();
    if (!token) return null;

    try {
      const decodedToken = jwtDecode<{ sub: string }>(token);
      return decodedToken.sub || null;
    } catch (error) {
      console.error('Invalid token::', error);
      return null;
    }
  }
  // --------------------------------------------------------------------------
}
