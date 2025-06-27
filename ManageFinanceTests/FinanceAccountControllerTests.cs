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
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;



public class FinanceAccountControllerTest
{
  private readonly FinanceAccountsController _controller;
  private readonly ApplicationDbContext _dbContext;

  public FinanceAccountControllerTest()
  { // Instantiate the configuration for an in-memory mock DB context:
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;

    // Create the in-memory mock DB context:
    _dbContext = new ApplicationDbContext(options);

    // Inject the mock in-memory context into the controller:
    _controller = new FinanceAccountsController(_dbContext);
  }



  [Fact]
  public async Task PostFinanceAccount_ShouldReturnCreatedAtAction_WhenUserExistAndIsAuthenticated()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email)
    };

    var identity = new ClaimsIdentity(claims, "Bearer");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      }
    };

    var financeAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    // Act: Call the function to test = PostFinanceAccount:
    var result = await _controller.PostFinanceAccount(financeAccount);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<FinanceAccount>>(result);
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
    Assert.Equal(201, createdAtActionResult.StatusCode);
    // Verify the newly created Finance Account was added into the in-memory DB:
    var retrievedAccount = await _dbContext.Accounts.FindAsync(financeAccount.Id);
    Assert.NotNull(retrievedAccount);
    Assert.Equal(retrievedAccount.Id, financeAccount.Id);
    Assert.Equal(retrievedAccount.AccountName, financeAccount.AccountName);
    Assert.Equal(retrievedAccount.AccountType, financeAccount.AccountType);
    Assert.Equal(retrievedAccount.Balance, 0);
    Assert.Equal(retrievedAccount.UserId, financeAccount.UserId);
  }



  [Fact]
  public async Task PostFinanceAccount_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = null
      }
    };

    var financeAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    // Act: Call the function to test = PostFinanceAccount:
    var result = await _controller.PostFinanceAccount(financeAccount);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<FinanceAccount>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }


  [Fact]
  public async Task PutFinanceAccount_ShouldReturnNoContent_WhenUserExistAndAuthenticatedAndAccountIdValid()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email)
    };

    var identity = new ClaimsIdentity(claims, "Bearer");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      }
    };

    var oldFinanceAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    var newFinanceAccount = new FinanceAccount
    {
      AccountName = "Private Account",
      AccountType = "Saving",
      UserId = "12345"
    };

    _dbContext.Accounts.Add(oldFinanceAccount);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = PutFinanceAccount:
    var result = await _controller.PutFinanceAccount(oldFinanceAccount.Id, newFinanceAccount);

    // Assert: Assess the response and status code:
    var noContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, noContentResult.StatusCode);

    // Retrieve the finance account from the in-memory DB to assess if its field has changed:
    var retrievedAccount = await _dbContext.Accounts.FindAsync(oldFinanceAccount.Id);
    Assert.Equal(retrievedAccount.Id, oldFinanceAccount.Id);
    Assert.Equal(retrievedAccount.AccountName, newFinanceAccount.AccountName);
    Assert.Equal(retrievedAccount.AccountType, newFinanceAccount.AccountType);
    Assert.Equal(retrievedAccount.Balance, oldFinanceAccount.Balance);
    Assert.Equal(retrievedAccount.UserId, newFinanceAccount.UserId); 
  }



  [Fact]
  public async Task PutFinanceAccount_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = null
      }
    };

    var oldFinanceAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    var newFinanceAccount = new FinanceAccount
    {
      AccountName = "Private Account",
      AccountType = "Saving",
      UserId = "12345"
    };

    _dbContext.Accounts.Add(oldFinanceAccount);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = PutFinanceAccount:
    var result = await _controller.PutFinanceAccount(oldFinanceAccount.Id, newFinanceAccount);

    // Assert: Assess the response and status code:
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }



  [Fact]
  public async Task PutFinanceAccount_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButIncorrectAccountId()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email)
    };

    var identity = new ClaimsIdentity(claims, "Bearer");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      }
    };


    var oldFinanceAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    var newFinanceAccount = new FinanceAccount
    {
      AccountName = "Private Account",
      AccountType = "Saving",
      UserId = "12345"
    };

    _dbContext.Accounts.Add(oldFinanceAccount);
    await _dbContext.SaveChangesAsync();

    var incorrectAccountId = "wkefh3o";

    // Act: Call the function to test = PutFinanceAccount:
    var result = await _controller.PutFinanceAccount(incorrectAccountId, newFinanceAccount);

    // Assert: Assess the response and status code:
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal(404, notFoundResult.StatusCode);
  }



  [Fact]
  public async Task DeleteFinanceAccount_ShouldReturnNoContent_WhenUserExistAndIsAuthenticated()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email)
    };

    var identity = new ClaimsIdentity(claims, "Bearer");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      }
    };


    var financeAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    _dbContext.Accounts.Add(financeAccount);
    await _dbContext.SaveChangesAsync();

    // Act: Assess the function to test = DeleteFinanceAccount:
    var result = await _controller.DeleteFinanceAccount(financeAccount.Id);

    // Assert: Assess the response and status code:
    var noContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, noContentResult.StatusCode);
  }




  [Fact]
  public async Task DeleteFinanceACcount_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = null
      }
    };

    var FinanceAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    _dbContext.Accounts.Add(FinanceAccount);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test:
    var result = await _controller.DeleteFinanceAccount(FinanceAccount.Id);

    // Assert: Assess the response and status code:
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }



  [Fact]
  public async Task DeleteFinanceAccount_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButInvallidId()
  { // Arrange: Set up the test environment:

    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email)
    };

    var identity = new ClaimsIdentity(claims, "Bearer");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      }
    };


    var financeAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 10.00M,
      UserId = "12345"
    };

    _dbContext.Accounts.Add(financeAccount);
    await _dbContext.SaveChangesAsync();

    var incorrectAccountId = "weriz3";

    // Act: Call the function to test = DeleteFinanceAccount:
    var result = await _controller.DeleteFinanceAccount(incorrectAccountId);

    // Assert: Assess the response and status code:
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal(404, notFoundResult.StatusCode);
  }




  [Fact]
  public async Task GetFinanceAccount_ShouldReturnOk_WhenUserExistAndAuthenticatedAndCorrectId()
  { // Arrange: Set up the test environment:

    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
      new Claim(ClaimTypes.Email, user.Email)
    };

    var identity = new ClaimsIdentity(claims, "Bearer");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      }
    };


    var financeAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 00.00M,
      UserId = "12345"
    };

    _dbContext.Accounts.Add(financeAccount);
    await _dbContext.SaveChangesAsync();


    // Act: Call the function to test = GetFinanceAccount:
    var result = await _controller.GetFinanceAccount(financeAccount.Id);

    // Assert: Assess the response and status code
    var actionResult = Assert.IsType<ActionResult<FinanceAccount>>(result);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    var returnedAccount = Assert.IsType<FinanceAccount>(okResult.Value);

    Assert.Equal(financeAccount.Id, returnedAccount.Id);
    Assert.Equal(financeAccount.AccountName, returnedAccount.AccountName);
    Assert.Equal(financeAccount.AccountType, returnedAccount.AccountType);
    Assert.Equal(financeAccount.Balance, 0);
    Assert.Equal(financeAccount.UserId, returnedAccount.UserId);

  }



  [Fact]
  public async Task GetFinanceAccount_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
  { // Arrange: Set up the test environment:


    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = null
      }
    };


    var financeAccount = new FinanceAccount
    {
      Id = "FinanceAccount1",
      AccountName = "Company Account",
      AccountType = "Saving",
      Balance = 00.00M,
      UserId = "12345"
    };

    _dbContext.Accounts.Add(financeAccount);
    await _dbContext.SaveChangesAsync();


    // Act: Call the function to test = GetFinanceAccount:
    var result = await _controller.GetFinanceAccount(financeAccount.Id);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<FinanceAccount>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }











}