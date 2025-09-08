import { HttpInterceptorFn } from '@angular/common/http';
import { TokenService } from './services/token.service';
import { inject } from '@angular/core';

// --------------------------------------------------------------------------
// Interceptor: Allows to intercept & modify HTTP requests and responses before they are sent or received:
// --------------------------------------------------------------------------

// --------------------------------------------------------------------------
// Functional interceptor: Allows to centrally add the Authorization header everywhere => make HTTP request secure and consistent:
// req: outgoing HTTP request (immutable so need to clone it to change something)
// next: function that passes the possibly modified request on to the next handler
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  // Read the token from the cookie:
  const cookie = tokenService.getCookie();
  if (cookie) {
    // Clone the request => add the token into the header as bearer:
    const cloned = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${cookie}`),
    });
    // Forwards the modified request to the backend:
    return next(cloned);
  }
  // Case where no cookie is found => send the request unmodified:
  return next(req);
};
// --------------------------------------------------------------------------
