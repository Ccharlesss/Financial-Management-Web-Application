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


public class InvestmentControllerTest
{
  private readonly InvestmentsController _controller;

  private readonly ApplicationDbContext _dbContext;

  public InvestmentControllerTest()
  { // Configure an in-memory DB:
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;

    // Create a DB context using the in-memory database to store & retrieve data during testing:
    _dbContext = new ApplicationDbContext(options);

    // Inject the in-memory DB into the InvestmentsController to run the tests:
    _controller = new InvestmentsController(_dbContext);
  }


  [Fact]
  public async Task PostInvestment_ShouldReturnCreatedAtAction_WhenUserExistAndIsAuthenticated()
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

    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };


    // Act: Call the function to test = PostInvestment:
    var result = await _controller.PostInvestment(investment);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Investment>>(result);
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
    Assert.Equal(201, createdAtActionResult.StatusCode);
    // Verify the goal was added into the in-memory DB:
    var savedInvestment = await _dbContext.Investments.FindAsync(investment.Id);
    Assert.NotNull(savedInvestment);
    Assert.Equal(investment.AssetName, savedInvestment.AssetName);
    Assert.Equal(investment.AmountInvested, savedInvestment.AmountInvested);
    Assert.Equal(investment.CurrentValue, savedInvestment.CurrentValue);
    Assert.Equal(investment.PurchaseDate, savedInvestment.PurchaseDate);
    Assert.Equal(investment.UserId, savedInvestment.UserId);
  }


  [Fact]
  public async Task PostInvestment_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
  { // Act: Set up the test environment:
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

    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    // Act: Call the function to test = PostInvestment:
    var result = await _controller.PostInvestment(investment);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Investment>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }




  [Fact]
  public async Task DeleteInvestment_ShouldReturnNoContent_WhenUserExistAndAuthenticated()
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

    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment);
    await _dbContext.SaveChangesAsync();


    // Act: Call the function to test = DeleteInvestment:
    var result = await _controller.DeleteInvestment(investment.Id);

    // Assert: Assess the response and status code:
    var NoContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, NoContentResult.StatusCode);
  }



  [Fact]
  public async Task DeleteInvestment_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
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


    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment);
    await _dbContext.SaveChangesAsync();


    // Act: Call the function to test = DeleteInvestment:
    var result = await _controller.DeleteInvestment(investment.Id);

    // Assert: Assess the response and the status code:
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }


  [Fact]
  public async Task DeleteInvestment_ShouldReturnNotFound_WhenUSerExistAndIsAuthenticatedButInvestmentIdIncorrect()
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

    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment);
    await _dbContext.SaveChangesAsync();

    var incorrectInvestmentId = "24u232";

    // Act: Call the function to test:
    var result = await _controller.DeleteInvestment(incorrectInvestmentId);

    // Assert: Assess the response and status code:
    var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal(404, NotFoundResult.StatusCode);
  }


  [Fact]
  public async Task PutInvestment_ShouldReturnNoContent_WhenUserExistAndAuthenticated()
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

    var oldInvestment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    var newInvestment = new Investment
    {
      AssetName = "Yacht",
      AmountInvested = 5000.00M,
      CurrentValue = 20.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };



    _dbContext.Investments.Add(oldInvestment);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test:
    var result = await _controller.PutInvestment(oldInvestment.Id, newInvestment);

    // Assert: Assess the response and status code:
    var noContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, noContentResult.StatusCode);
    // Retrieve the object in the database to see if the fields were modified:
    var retrievedInvestment = await _dbContext.Investments.FindAsync(oldInvestment.Id);
    Assert.Equal(retrievedInvestment.AssetName, newInvestment.AssetName);
    Assert.Equal(retrievedInvestment.AmountInvested, newInvestment.AmountInvested);
    Assert.Equal(retrievedInvestment.CurrentValue, newInvestment.CurrentValue);
    Assert.Equal(retrievedInvestment.PurchaseDate, newInvestment.PurchaseDate);
    Assert.Equal(retrievedInvestment.UserId, newInvestment.UserId);
  }


  [Fact]
  public async Task PutInvestment_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
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

    var oldInvestment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    var newInvestment = new Investment
    {
      AssetName = "Yacht",
      AmountInvested = 5000.00M,
      CurrentValue = 20.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };



    _dbContext.Investments.Add(oldInvestment);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test:
    var result = await _controller.PutInvestment(oldInvestment.Id, newInvestment);

    // Assert: Assess the response and status code:
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }



  [Fact]
  public async Task PutInvestment_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButIncorrectId()
  {
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

    var oldInvestment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    var newInvestment = new Investment
    {
      AssetName = "Yacht",
      AmountInvested = 5000.00M,
      CurrentValue = 20.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(oldInvestment);
    await _dbContext.SaveChangesAsync();

    var incorrectId = "232hu";

    // Act: Call the function to test:
    var result = await _controller.PutInvestment(incorrectId, newInvestment);

    // Assert: Assess the response and status code:
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal(404, notFoundResult.StatusCode);
  }


  [Fact]
  public async Task GetInvestment_ShouldReturnOk_WhenUserExistAndAuthenticated()
  { // 1) Arrange: Set up the test environment:
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

    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = GetInvestment:
    var result = await _controller.GetInvestment(investment.Id);

    // Assert: Assess the response and status code
    var actionResult = Assert.IsType<ActionResult<Investment>>(result);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    var returnedInvestment = Assert.IsType<Investment>(okResult.Value);

    Assert.Equal(investment.Id, returnedInvestment.Id);
    Assert.Equal(investment.AssetName, returnedInvestment.AssetName);
    Assert.Equal(investment.AmountInvested, returnedInvestment.AmountInvested);
    Assert.Equal(investment.CurrentValue, returnedInvestment.CurrentValue);
    Assert.Equal(investment.PurchaseDate, returnedInvestment.PurchaseDate);
    Assert.Equal(investment.UserId, returnedInvestment.UserId);
  }



  [Fact]
  public async Task GetInvestment_ShouldReturnUnauthorized_WhenUserExistAndNotAuthenticated()
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

    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = GetInvestment:
    var result = await _controller.GetInvestment(investment.Id);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Investment>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }


  [Fact]
  public async Task GetInvestment_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButIncorrectInvestmentId()
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

    var investment = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment);
    await _dbContext.SaveChangesAsync();

    var incorrectId = "wer23o8";

    // Act: Call the function to test = GetInvestment:
    var result = await _controller.GetInvestment(incorrectId);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Investment>>(result);
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
    Assert.Equal(404, notFoundResult.StatusCode);
  }




  [Fact]
  public async Task GetInvestments_ShouldReturnOk_WhenUserExistAndAuthenticated()
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

    var investment1 = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    var investment2 = new Investment
    {
      Id = "Investment2",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment1);
    _dbContext.Investments.Add(investment2);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test: GetInvestments:
    var result = await _controller.GetInvestments();

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<IEnumerable<Investment>>>(result);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    Assert.Equal(200, okResult.StatusCode);
    // Extract the list from Okresult:
    var returnedInvestments = Assert.IsType<List<Investment>>(okResult.Value);
    // Look at the count and values:
    Assert.Equal(2, returnedInvestments.Count);
    Assert.Contains(returnedInvestments, g => g.Id == "Investment1" && g.AssetName == "Mansion");
    Assert.Contains(returnedInvestments, g => g.Id == "Investment2" && g.AssetName == "Mansion");
  }


  [Fact]
  public async Task GetInvestments_ShouldReturnUnauthorized_WhenUserExistButIsNotAuthenticated()
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

    var investment1 = new Investment
    {
      Id = "Investment1",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    var investment2 = new Investment
    {
      Id = "Investment2",
      AssetName = "Mansion",
      AmountInvested = 16000.00M,
      CurrentValue = 20000.99M,
      PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
      UserId = "12345"
    };

    _dbContext.Investments.Add(investment1);
    _dbContext.Investments.Add(investment2);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test: GetInvestments:
    var result = await _controller.GetInvestments();
    
    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<IEnumerable<Investment>>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);

  }




















}