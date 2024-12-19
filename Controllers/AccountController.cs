// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.IdentityModel.Tokens;

// namespace ManageFinance.Controllers
// {
//   [Route("api/[controller]")]
//   [ApiController]
  
//   public class AccountController : ControllerBase
//   {
//     private readonly UserManager<IdentityUser> _userManager; // Task: Manages user's account operations:
//     private readonly SignInManager<IdentityUser> _signInManager; // Task: Manage sign-in / sign-out operations:
//     private readonly EmailService _emailService; // Task: Send Email w/ verification link:

//     private readonly IConfiguration _configuration;


//     // Constructor: Instantiate instances of UserManager, SignInManager & EmailService:
//     public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, EmailService emailservice,
//     IConfiguration configuration)
//     {
//       _userManager = userManager;
//       _signInManager = signInManager;
//       _emailService = emailservice;
//       _configuration = configuration;
//     }

//     // ==================================================================================================================================
//     // Purpose: Register new users in the system:
//     [HttpPost("register")]
//     public async Task<IActionResult> Register(AuthSchema model)
//     { // Create an instance of user of type 'IdentityUser' which uses email (username) and password:
//       var user = new IdentityUser { UserName = model.Email, Email = model.Email};
//       // Attempts to create a new user in the system using the 'UserManager<IdentityUser>' service:
//       // Returns 'IdentityResult' which indicates if user registration was successful:
//       var result = await _userManager.CreateAsync(user,model.Password);

//       if(result.Succeeded)
//       { // Generate an email verification token to the newly registered user:
//         var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//         // Create the verification link:
//         var verificationLink = Url.Action("VerifyEmail", "Account", new{userId = user.Id, token=token}, Request.Scheme);
//         // Send the verification Email to the new user:
//         var emailSubject = "Email Verification";
//         var emailBody = $"Please verify your email by clicking the following link: {verificationLink}";
//         _emailService.SendEmail(user.Email, emailSubject, emailBody);
//         // Return an HTTP 200 Ok if user has been registered successfully into the syytem:
//         return Ok("User registered successfully. An email verification link has been sent.");
//       }

//       // Returns an HTTP 400 Bad Request if User couldn't be registered into the system:
//       return BadRequest(result.Errors);
//     }
//     // ==================================================================================================================================





//     // ==================================================================================================================================
//     // Purpose: Account confirmation:
//     [HttpGet("verify-email")]
//     public async Task<IActionResult> VerifyEmail(string userId, string token)
//     { // Retrieve the user corresponding to the userId:
//       var user = await _userManager.FindByIdAsync(userId);

//       // Case where user has not been found => Return 404 not found:
//       if(user == null)
//       {
//         return NotFound("User not found.");
//       }

//       // Case where user has been found:
//       // Attempts to confirm the email for the speciied user using the provided token to confirm the account:
//       var result = await _userManager.ConfirmEmailAsync(user, token);
//       // Case where emailVerification was successfull => Return an HTTP 200 Ok:
//       if(result.Succeeded)
//       {
//         return Ok("Email verification successful.");
//       }
//       // Case where emailVerification was unsuccessfull => Return a 400 BadRequest:
//       return BadRequest("Email verification failed.");
//     }
//     // ==================================================================================================================================






//     // ==================================================================================================================================
//     // Purpose: Enable the user to login to his account:
//     [HttpPost("login")] // api/Account/login.
//     public async Task<IActionResult> Login(AuthSchema model) // Parameters: User Email (username) and Password
//     {
//       // Attempt to login using user's credentials (email & password):
//       // isPersistent = false: Specifies that the user's sign-in status should not persist across browser sessions:
//       // lockoutOnFailure = false: Specifies that the account should not be locked out in case of failed login attempt:
//       var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password,isPersistent: false, lockoutOnFailure: false);

//       // Returns a 200 OK if login successful + JWT payload:
//       if (result.Succeeded)
//       {   // Returns a 200 OK if login successful
//           var user = await _userManager.FindByEmailAsync(model.Email);
//           var roles = await _userManager.GetRolesAsync(user);
//           var token = GenerateJwtToken(user,roles);
//           return Ok(new { Token = token });
//       }
            
//       // Returns a 401 Unauthorized if login unsuccessful
//       return Unauthorized("Invalid login attempt.");
//     }
//     // ==================================================================================================================================




//     // ==================================================================================================================================
//     // Purpose: Handle the logout:
//     [HttpPost("logout")]
//     public async Task<IActionResult> Logout()
//     {
//       await _signInManager.SignOutAsync(); // Log out the user
//       return Ok("Logged out.");
//     }
//     // ==================================================================================================================================




//     // ==================================================================================================================================
//     // Purpose: Generates the JWT token:
//     private string GenerateJwtToken(IdentityUser user, IList<string> roles)
//     {
//       // initializes a new list of Claim objects
//       var claims = new List<Claim> 
//       { // Adds 2 claims to the list:
//         new Claim(JwtRegisteredClaimNames.Sub, user.Email), // The subject of the claim = user's email.
//         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID claim = unique IF for the token.
//       };

//       // Each permissions the user possess will be added to the list claim
//       foreach (var role in roles)
//       {
//         claims.Add(new Claim(ClaimTypes.Role, role));
//       }

//       var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // Generate the key
//       var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Generate the signature
//       var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"])); // Generate expiration time

//       // New JwtSecurityToken object is created => Header is automatically generated
//       var token = new JwtSecurityToken(
//         _configuration["Jwt:Issuer"], // Payload -> Issuer: The entity that issued the JWT = WebApp
//         _configuration["Jwt:Issuer"], // Payload -> Audience: Who the token is issued to
//         claims, // Payload -> Claims
//         expires: expires, // Payload -> Expiration date 
//         signingCredentials: creds // Signature
//       );
//       // Returns the JWT generated as a string w/ JwtSecurityTokenHandler to serialize the token into a string representation.
//       return new JwtSecurityTokenHandler().WriteToken(token);
//     }
//     // ==================================================================================================================================






//     // ==================================================================================================================================
//     // Purpose: Reset user's password:
//     [HttpPut]
//     public async Task<IActionResult> ChangePassword(UpdatePasswordSchema data)
//     {
//       // Retrieve the user from the provided email:
//       var user = await _userManager.FindByEmailAsync(data.Email);

//       // Case where no user could be retrieved from the email:
//       if(user == null)
//       {
//         return NotFound("Invalid Email. No user could be retrieved.");
//       }

//       // Attempt to update user's password:
//       // Generate a password reset token:
//       var token = await _userManager.GeneratePasswordResetTokenAsync(user);
//       // Reset the user's password to the new one using the token as permission:
//       var result = await _userManager.ResetPasswordAsync(user,token, data.NewPassword);

//       if(result.Succeeded)
//       {
//         return Ok("Password changed successfully.");
//       }

//       return BadRequest(result.Errors);
//     }
//   }
// }




using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ManageFinance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager; // Manages user's account operations
        private readonly SignInManager<AppUser> _signInManager; // Manages sign-in / sign-out operations
        private readonly EmailService _emailService; // Sends Email with verification link
        private readonly IConfiguration _configuration;

        // Constructor: Instantiate instances of UserManager, SignInManager & EmailService
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, EmailService emailservice, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailservice;
            _configuration = configuration;
        }

        // ==================================================================================================================================
        // Purpose: Register new users in the system
        // [HttpPost("register")]
        // public async Task<IActionResult> Register(AuthSchema model)
        // {
        //     // Create an instance of user of type 'AppUser' which uses email (username) and password
        //     var user = new AppUser { UserName = model.Email, Email = model.Email };

        //     // Attempts to create a new user in the system using the 'UserManager<AppUser>' service
        //     var result = await _userManager.CreateAsync(user, model.Password);

        //     if (result.Succeeded)
        //     {
        //         // Generate an email verification token to the newly registered user
        //         var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //         // Create the verification link
        //         var verificationLink = Url.Action("VerifyEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
        //         // Send the verification Email to the new user
        //         var emailSubject = "Email Verification";
        //         var emailBody = $"Please verify your email by clicking the following link: {verificationLink}";
        //         _emailService.SendEmail(user.Email, emailSubject, emailBody);
        //         // Return an HTTP 200 Ok if user has been registered successfully into the system
        //         return Ok("User registered successfully. An email verification link has been sent.");
        //     }

        //     // Returns an HTTP 400 Bad Request if User couldn't be registered into the system
        //     return BadRequest(result.Errors);
        // }


        
        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthSchema model)
        {   

            // Create an instance of user of type 'AppUser' which uses email (username) and password
            var user = new AppUser { UserName = model.Email, Email = model.Email };

            // Attempts to create a new user in the system using the 'UserManager<AppUser>' service
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Ensure the role exist in the AspNetRole table:
                var roleExist = await _userManager.IsInRoleAsync(user, "User");
                // Case where the role was not assigned to the user:
                if(!roleExist){

                    // Assign the role = "User" to the newly created user:
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");

                    // Case where the Assignment of the role didn't succeed:
                    if(!roleResult.Succeeded){
                        return BadRequest("Failed to assign the role 'User' to");
                    }
                }

                // Generate an email verification token to the newly registered user
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //URL-encode the token to make it safe to use in a URL query string:
                var encodedToken = System.Net.WebUtility.UrlEncode(token);
                // Create the verification link with the encoded token
                var verificationLink = Url.Action("VerifyEmail", "Account", new { userId = user.Id, token = encodedToken }, Request.Scheme);
                // Send the verification Email to the new user
                var emailSubject = "Email Verification";
                var emailBody = $"Please verify your email by clicking the following link: {verificationLink}";
                _emailService.SendEmail(user.Email, emailSubject, emailBody);
                // Return an HTTP 200 Ok if user has been registered successfully into the system
                return Ok("User registered successfully. An email verification link has been sent.");
            }

            // Returns an HTTP 400 Bad Request if User couldn't be registered into the system
            return BadRequest(result.Errors);
        }



        // ==================================================================================================================================

        // ==================================================================================================================================
        // Purpose: Account confirmation
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            // URL-decode the token receibed in the query string:
            var decodedToken = System.Net.WebUtility.UrlDecode(token);
            // Retrieve the user corresponding to the userId
            var user = await _userManager.FindByIdAsync(userId);

            // Case where user has not been found => Return 404 not found
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Case where user has been found:
            // Attempts to confirm the email for the specified user using the provided token
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            // Case where email verification was successful => Return an HTTP 200 Ok
            if (result.Succeeded)
            {
                return Ok("Email verification successful.");
            }
            // Case where email verification was unsuccessful => Return a 400 BadRequest
            return BadRequest("Email verification failed.");
        }
        // ==================================================================================================================================

        // ==================================================================================================================================
        // Purpose: Enable the user to login to their account
        [HttpPost("login")] // api/Account/login
        public async Task<IActionResult> Login(AuthSchema model) // Parameters: User Email (username) and Password
        {
            // Attempt to login using user's credentials (email & password)
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            // Returns a 200 OK if login successful + JWT payload
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                var token = GenerateJwtToken(user, roles);
                return Ok(new { Token = token });
            }

            // Returns a 401 Unauthorized if login unsuccessful
            return Unauthorized("Invalid login attempt.");
        }
        // ==================================================================================================================================

        // ==================================================================================================================================
        // Purpose: Handle the logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Log out the user
            return Ok("Logged out.");
        }
        // ==================================================================================================================================

        // ==================================================================================================================================
        // Purpose: Generates the JWT token
        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            // Initializes a new list of Claim objects
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email), // The subject of the claim = user's email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID claim = unique ID for the token
            };

            // Each permission the user has will be added to the claims list
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // Generate the key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Generate the signature
            var expires = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"])); // Generate expiration time

            // Create and return the JWT as a string
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"], // Payload -> Issuer: The entity that issued the JWT = WebApp
                _configuration["Jwt:Issuer"], // Payload -> Audience: Who the token is issued to
                claims, // Payload -> Claims
                expires: expires, // Payload -> Expiration date 
                signingCredentials: creds // Signature
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // ==================================================================================================================================

        // ==================================================================================================================================
        // Purpose: Reset user's password
        [HttpPut]
        public async Task<IActionResult> ChangePassword(UpdatePasswordSchema data)
        {
            // Retrieve the user from the provided email
            var user = await _userManager.FindByEmailAsync(data.Email);

            // Case where no user could be retrieved from the email
            if (user == null)
            {
                return NotFound("Invalid Email. No user could be retrieved.");
            }

            // Attempt to update user's password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, data.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }

            return BadRequest(result.Errors);
        }
        // ==================================================================================================================================
    }
}
