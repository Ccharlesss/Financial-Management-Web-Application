using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using ManageFinance.Controllers;
using ManageFinance.Services;
using ManageFinance.Schemas;
using ManageFinance.Response;
using Microsoft.AspNetCore.Mvc;
using ManageFinance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
    private readonly Mock<IUrlHelper> _mockUrlHelper; // Service thatgenerates URLs ex: for verification links:
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        // A) Purpose: Mock UserManager dependencies:
        // A.1) Mock the interface that UserManager uses to interact w/ DB:
        var store = new Mock<IUserStore<AppUser>>();
        // A.2) Instantiate the mock UserManager:
        _mockUserManager = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        // B) Purpose: Mock SignInManager dependencies:
        // B.1) Mock the HttpContext:
        var contextAccessor = new Mock<IHttpContextAccessor>();
        // B.2) Mock ClaimsPrincipal used to represent the user's identity and role
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
        // B.3) Instantiate the mock SignInManager:
        _mockSignInManager = new Mock<SignInManager<AppUser>>(
            _mockUserManager.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

        // C) Mock other dependencies
        _mockEmailService = new Mock<IEmailService>();
        _mockUrlHelper = new Mock<IUrlHelper>();
        _mockJwtService = new Mock<IJwtService>();

        // D) Initialize the controller with mocked dependencies
        _controller = new AccountController(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockEmailService.Object,
            _mockUrlHelper.Object,
            _mockJwtService.Object
        );

        // E) Mock HttpContext and Request
        // E.1) Mock HttpContext: context for the current request:
        var httpContext = new Mock<HttpContext>();
        // E.2) Mock Request:
        var request = new Mock<HttpRequest>();
        // E.3) Request.Scheme is used in the AccountController to generate verification link => Set the scheme to "https":
        request.Setup(r => r.Scheme).Returns("https");
        httpContext.Setup(h => h.Request).Returns(request.Object);

        // F) Assign the mocked HttpContext to the controller:
        _controller.ControllerContext = new ControllerContext
        {   // F.1) Assign the mocked HttpContext to the AccountController context:
            HttpContext = httpContext.Object,
            // F.2) Represent the action being executed:
            ActionDescriptor = new ControllerActionDescriptor()
        };
    }

    [Fact]
    public async Task Register_ShouldReturnOK_WhenUserIsCreatedSuccessfully()
    {   // Arrange: Sets up the test environment:
        var userModel = new AuthSchema
        {
            Email = "test@example.com",
            Password = "MySecretPassword64$"
        };

        // Mock UserManager.CreateAsync to return success:
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        // Mock UserManager.IsInRoleAsync to return false (role does not exist):
        _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(false);

        // Mock UserManager.AddToRoleAsync to return success:
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        // Mock UserManager.GenerateEmailConfirmationTokenAsync to return a token:
        _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<AppUser>()))
                        .ReturnsAsync("dummy-token");

        // Mock EmailService.SendEmail to do nothing (verifiable):
        _mockEmailService.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                         .Verifiable();

        // Mock Url.Action to return a dummy verification link:
        _mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                     .Returns("http://dummy-verification-link");

        // Act: Calls the method being tested:
        var result = await _controller.Register(userModel);

        // Assert: Test the response:
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        // Verify that SendEmail was called once:
        _mockEmailService.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }



    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserWithSameEmailAddressAlreadyExist()
    {   // Arange: Set up the test environment:
        var same_username_user = new AuthSchema{Email = "test@example.com", Password = "MySecretPassword64$"};

        // Mock UserManager.CreateAsync to simulate email already in use:
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError {Description = $"Username {same_username_user.Email} is already taken."}));

        // Act: Call the Register method:
        var result = await _controller.Register(same_username_user);

        // Assert: Ensure it returns a BadRequestObjectResult:
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }



    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenFailedToAssignRoleUser()
    {   // Arrange: Set up the test environment:
        var signUpData = new AuthSchema{Email = "test@example.com", Password = "MySecretPassword64$"};

        // Mock UserManager CreateAsync to return success:
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        // Mock UserManager IsInRoleAsync to return false:
        _mockUserManager.Setup(x => x.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Mock UserManager AddToRoleAsync to return failure:
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError {Description = "Failed to assign role."}));

        // Act: Call the Function to test:
        var result = await _controller.Register(signUpData);

        // Assert: Ensure it returns a BadRequestObjectResult with the correct error message:
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Failed to assign the role 'User' to the user.", badRequestResult.Value);
    }



    [Fact]
    public async Task VerifyEmail_ShouldReturnOk_WhenEmailVerificationIsSuccessful()
    {   // Arrange: Set up the test encvironment:
        var userId = "12345";
        var validToken = "valid-token";
        var user = new AppUser {Id = userId, Email = "test@example.com"};

        // Mock UserManager FindByIdAsync to return the user:
        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        
        // Mock UserManager ConfirmEmailAsync to return success:
        _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act: Call the VerifyEmail method to test:
        var result = await _controller.VerifyEmail(userId, validToken);

        // Assert: Ensure it returns 200 OK:
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal("Email verification successful.", okResult.Value);
    }


    [Fact]
    public async Task VerifyEmail_ShouldReturnNotFound_WhenUserDoesNotExist()
    {   // Arrange: Set up the test environment:
        var userId = "12345";
        var validToken = "valid-token";
        var user = new AppUser {Id = userId, Email = "test@example.com"};

        // Mock UserManager FindByIdAsync to return user not found:
        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((AppUser)null);
        
        // Act: Call the function to test = VerifyEmail:
        var result = await _controller.VerifyEmail(userId, validToken);

        // Assert: Ensures it returns a 404 not found:
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal("User not found.", notFoundResult.Value);
    }



    [Fact]
    public async Task VerifyEmail_ShouldReturnBadRequest_WhenEmailVerificationFailed()
    {   // Arrange: Set up the test environment:
        var userId = "12345";
        var invalidToken = "invalid-token";
        var user = new AppUser {Id = userId, Email = "example@test.com"};

        // Mock UserManager FindByIdAsync to return a user:
        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        // Mock UserManager ConfirmEmail to fail:
        _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError {Description = "Invalid token"}));

        // Act: Call the function to test = VerifyEmail:
        var result = await _controller.VerifyEmail(userId, invalidToken);

        // Assert: Ensures it returns a 400 badRequest:
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Email verification failed.", badRequestResult.Value);
    }



    [Fact]
    public async Task Login_ShouldReturnOkWithToken_WhenEmailAndPasswordAreCorrect()
    {   // Arrange: Set up the test environment:
        var userId = "12345";
        var userEmail = "example@test.com";
        var userPassword = "StrongPassword64$";
        var user = new AppUser {Id = userId, Email = "example@test.com"};
        var userCredentials = new AuthSchema {Email = userEmail, Password = userPassword};

        // Mock SignInManager PasswordSignInAsync to return success:
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(userEmail, userPassword, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Mock UserManager FindByEmailAsync to return the user:
        _mockUserManager.Setup(x => x.FindByEmailAsync(userEmail))
            .ReturnsAsync(user);

        // Mock UserManager GetRolesAsync to return roles assigned to the user:
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> {"User"});

        // Mock GenerateJWT token to return the token:
        var expectedToken = "mock-JWT-token";
        _mockJwtService.Setup(x => x.GenerateJwtToken(user, It.IsAny<IList<string>>()))
            .Returns(expectedToken);
        
        // Act: Call the Login function to test:
        var result = await _controller.Login(userCredentials);

        // Assert: Assess that the login was successful:
        var OkResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, OkResult.StatusCode);
        // Extract the response value and check if it contains the expected token
        var response = OkResult.Value as LoginResponse;
        Assert.NotNull(response);
        Assert.Equal(expectedToken, response.Token);
    }



    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {   // Arrange: Set up the test environment:
        var userEmail = "example@test.com";
        var incorrectPassword = "WrongPassword";
        var incorrectCredentials = new AuthSchema {Email = userEmail, Password = incorrectPassword};
    
        // Mock SignInManager PasswordSignInAsync to return failure:
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(userEmail, incorrectPassword, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act: Call the Login function to test:
        var result = await _controller.Login(incorrectCredentials);
        
        // Assert: Assess that the login was unsuccessfull:
        var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, UnauthorizedResult.StatusCode);
        Assert.Equal("Invalid login attempt.", UnauthorizedResult.Value);
    }



    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenEmailIsIncorrect()
    {   // Arrange: Set up the test environment:
        var incorrectEmail = "wrongEmail@test.com";
        var userPassword = "StrongPassword64$";
        var incorrectCredentials = new AuthSchema {Email = incorrectEmail, Password = userPassword};

        // Mock SignInManager PasswordSignInAsync to return failure:
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(incorrectEmail, userPassword, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act: Call the login function to test:
        var result = await _controller.Login(incorrectCredentials);

        // Assert: Assess that the login was unsuccessful:
        var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, UnauthorizedResult.StatusCode);
        Assert.Equal("Invalid login attempt.", UnauthorizedResult.Value);
    }



    [Fact]
    public async Task Logout_ShouldReturnOk()
    {   // Arrange: Set up the test environment:
        _mockSignInManager.Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);
        
        // Act: Call the function to test: Logout:
        var result = await _controller.Logout();
        
        // Assert: Assess that the logout was successful:
        var OkResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, OkResult.StatusCode);
        Assert.Equal("Logged out.", OkResult.Value);
    }


    [Fact]
    public async Task ChangePassword_ShouldReturnOk_WhenEmailandNewPasswordValid()
    {   // Arrange: Set up the test environment:
        var userId = "12345";
        var userEmail = "example@test.com";
        var newPassword = "newPassword64$";
        var user = new AppUser {Id = userId, Email = userEmail};

        var newCredentials = new UpdatePasswordSchema {Email = userEmail, NewPassword = newPassword};

        // Mock UserManager FindbyEmailAsync to return success:
        _mockUserManager.Setup(x => x.FindByEmailAsync(userEmail))
            .ReturnsAsync(user);

        // Mock UserManager GeneratePasswordResetTokenAsync to return a token:
        _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync("dummy-Token");
        
        // Mock ResetPasswordAsynx to return success:
        _mockUserManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), newPassword))
            .ReturnsAsync(IdentityResult.Success);
        
        // Act: Call the function to test = ChangePassword():
        var result = await _controller.ChangePassword(newCredentials);

        // Assert: Assess that the password was successfully changed:
        var OkResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, OkResult.StatusCode);
        Assert.Equal("Password changed successfully.", OkResult.Value);
    }



    [Fact]
    public async Task ChangePassword_ShouldReturnNotFound_WhenIncorrectEmail()
    {   // Act: Set up the test environment:
        var wrongUserEmail = "wrong@Test.com";
        var newCredential = new UpdatePasswordSchema {Email = wrongUserEmail, NewPassword = "NewPassword"};

        // Mock UserManager FindByEmailAsync to return failure:
        _mockUserManager.Setup(x => x.FindByEmailAsync(wrongUserEmail))
            .ReturnsAsync((AppUser)null);
        
        // Act: Call the function to test = ChangePassword:
        var result = await _controller.ChangePassword(newCredential);

        // Assert: Assess that ChangePassword fails:
        var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, NotFoundResult.StatusCode);
        Assert.Equal("Invalid Email. No user could be retrieved.", NotFoundResult.Value);
    }



    [Fact]
    public async Task ChangePassword_ShouldReturnBadRequest_WhenPasswordResetFails()
    {   // Act: Set up the test environment:
        var userEmail = "example@test.com";
        var user = new AppUser {Id = "12345", Email = userEmail};
        var newPassword = "newPassword";
        var newCredential = new UpdatePasswordSchema {Email = userEmail, NewPassword = newPassword};

        // Moch UserManager FindByEmailAsync to return success:
        _mockUserManager.Setup(x => x.FindByEmailAsync(userEmail))
            .ReturnsAsync(user);
        
        // Mock UserManager GeneratePasswordResetTokenAsync to return a token:
        _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync("dummy-token");

        // Mock UserManager ResetPasswordAsynx to return fail:
        var failedResult = IdentityResult.Failed(new IdentityError {Description = "Invalid Token."});
        _mockUserManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), newPassword))
            .ReturnsAsync(failedResult);
        
        // Act: Call the function to test = ChangePassword()
        var result = await _controller.ChangePassword(newCredential);

        // Assert: Assess that ChangePassword() returns BadRequest:
        var BadRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, BadRequestResult.StatusCode);
        Assert.Equal("Invalid Token.", (BadRequestResult.Value as IEnumerable<IdentityError>).First().Description);
    }

}