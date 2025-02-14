    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;


namespace ManageFinance.Services
{
    // Define an Interface for the JWT generation service:
    public interface IJwtService
    {
        string GenerateJwtToken(AppUser user, IList<string> roles);
    }


    // Class that implements the JWT service:
    public class JwtService : IJwtService
    {
        // Inject IConfiguration to enable to access settings from appsettings.json:
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(AppUser user, IList<string> roles)
        {   // 1) Generate a Secret Key: Convers the secret key from appsettings.json into bytes:
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            // 2) Create signin credentials: use key to create digital signature and encrypt it using HmacSha256 algorithm:
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3) Create claims:
            var claims = new List<Claim>
            {   // Stores user info in the claim:
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // 4) Add user roles as claims: Adds each roles of the user in the claim
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // 5) Generate the JWT token:
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // Issuer: Who created the token
                audience: _configuration["Jwt:Audience"], // Audience: Who can use the token
                claims: claims, // Claims
                expires: DateTime.UtcNow.AddHours(1), // Validity period (expiration)
                signingCredentials: credentials // Digital signature to prevent tampering
            );

            // 6) Return the JWT token as a string:
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
