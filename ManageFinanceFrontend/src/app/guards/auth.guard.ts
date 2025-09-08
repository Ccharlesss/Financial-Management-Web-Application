import { CanActivateFn, Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  // Get the token from the cookie:
  const token = tokenService.getCookie();
  if (token) {
    return true;
  }

  // Case where no token => Not authenticated => redirect unauthenticated user to loggin:
  router.navigate(['/signin']);
  return false;
};
