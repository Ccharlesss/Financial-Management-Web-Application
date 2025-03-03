    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;


namespace ManageFinance.Services;

public interface IJwtService
{
  string GenerateJwtToken(AppUser user, IList<string> roles);

}



public class TokenService : IJwtService
{

  private readonly IConfiguration _configuration;

  public TokenService(IConfiguration configuration)
  {
    _configuration = configuration;
  }
  public string GenerateJwtToken(AppUser user, IList<string> roles)
  { // Initialize a new list of Claim objects:
    // Claims are Key-value pairs that stores information about a user to represent its identity & priviledges:
    var claims = new List<Claim>
    { // UserId:
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      // Subject of the claim = user's email:
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
      // JWT ID = unique ID of the JWT token:
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    // Each permission the user has will be added to the claims list:
    foreach(var role in roles)
    {
      claims.Add(new Claim(ClaimTypes.Role, role));
    }

    // Generate the key:
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    // Generate the signature:
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    // Generate the expiration time:
    var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"]));

    // Create the token & return the JWT as a string:
    var token = new JwtSecurityToken(
      issuer: _configuration["Jwt:Issuer"], // Payload -> Issuer: The entity that issued the JWT = WebApp
      audience: _configuration["Jwt:Issuer"], // Payload -> Audience: Who the token is issued to
      claims: claims, // Payload -> Claims
      expires: expires, // Payload -> Expiration date 
      signingCredentials: creds // Signature
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}











