export interface JwtPayLoad {
  // Subject: UserId:
  sub: string;
  // User Email:
  email: string;
  // TokenID:
  jti: string;
  // Expiration:
  exp: number;
  // issued at:
  iat: number;
  // Role(s) of the user:
  role: string | string[];
}
