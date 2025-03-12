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

public class BudgetControllerTest
{
  private readonly BudgetsController _controller;
  private readonly ApplicationDbContext _dbContext;


  public BudgetControllerTest()
  { // Configure an in-memory DB:
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;

    // Create the mock DBcontext using the in-memory configuration DB:
    _dbContext = new ApplicationDbContext(options);

    // Inject the in-memory DB into the controller:
    _controller = new BudgetsController(_dbContext);
  }



  [Fact]
  public async Task PostBudget_ShouldReturnCreatedAtAction_WhenUserExistAndAuthenticated()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    // Act: Call the function to test = PostBudget:
    var result = await _controller.PostBudget(budget);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Budget>>(result);
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
    Assert.Equal(201, createdAtActionResult.StatusCode);

    // Verify the budget was saved into the DB:
    var savedBudget = await _dbContext.Budgets.FindAsync(budget.Id);
    Assert.NotNull(savedBudget);
    Assert.Equal(budget.Id, savedBudget.Id);
    Assert.Equal(budget.Category, savedBudget.Category);
    Assert.Equal(budget.Limit, savedBudget.Limit);
    Assert.Equal(budget.UserId, savedBudget.UserId);
  }



  [Fact]
  public async Task PostBudget_ShouldReturnUnauthorized_WhenUserExistAndIsNotAuthenticated()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    // Act: Call the function to test = PostBudget:
    var result = await _controller.PostBudget(budget);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Budget>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }


  [Fact]
  public async Task PutBudget_ShouldReturnNoContent_WhenUserExistAndAuthenticated()
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


    var oldBudget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    var newBudget = new Budget
    {
      Category = "Sport",
      Limit = 5000.00M,
    };

    _dbContext.Budgets.Add(oldBudget);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = PostBudget:
    var result = await _controller.PutBudget(oldBudget.Id, newBudget);    

    // Assert: Assess the response and status code:
    var noContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, noContentResult.StatusCode);
    // Retrieve the object in the database to see if the fields were modified:
    var retrievedBudget = await _dbContext.Budgets.FindAsync(oldBudget.Id);
    Assert.Equal(retrievedBudget.Category, newBudget.Category);
    Assert.Equal(retrievedBudget.Limit, newBudget.Limit);
  }



  [Fact]
  public async Task PutInvestment_ShouldReturnUnauthorized_WhenUserExistButIsNotAuthenticated()
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


    var oldBudget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    var newBudget = new Budget
    {
      Category = "Sport",
      Limit = 5000.00M,
    };

    _dbContext.Budgets.Add(oldBudget);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = PostBudget:
    var result = await _controller.PutBudget(oldBudget.Id, newBudget);    

    // Assert: Assess the response and status code:
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }



  [Fact]
  public async Task PutBudget_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButIncorrectBudgetId()
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


    var oldBudget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    var newBudget = new Budget
    {
      Category = "Sport",
      Limit = 5000.00M,
    };

    _dbContext.Budgets.Add(oldBudget);
    await _dbContext.SaveChangesAsync();

    var incorrectBudgetId = "wdkfh32";

    // Act: Call the function to test = PostBudget:
    var result = await _controller.PutBudget(incorrectBudgetId, newBudget);    

    // Assert: Assess the response and status code:
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal(404, notFoundResult.StatusCode);
  }




  [Fact]
  public async Task DeleteBudget_ShouldReturnNoContent_WhenUserExistAndAuthenticated()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    _dbContext.Budgets.Add(budget);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test:
    var result = await _controller.DeleteBudget(budget.Id);

    // Assert: Assess the response and status code:
    var noContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, noContentResult.StatusCode);
  }



  [Fact]
  public async Task DeleteBudget_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    _dbContext.Budgets.Add(budget);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test:
    var result = await _controller.DeleteBudget(budget.Id);

    // Assert: Assess the response and status code:
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }



  [Fact]
  public async Task DeleteBudget_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButIncurrectBudgetId()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    _dbContext.Budgets.Add(budget);
    await _dbContext.SaveChangesAsync();

    var incorrectBudgetId = "wekfhw";

    // Act: Call the function to test:
    var result = await _controller.DeleteBudget(incorrectBudgetId);

    // Assert: Assess the response and status code:
    var notfoundResult = Assert.IsType<NotFoundResult>(result);
    Assert.Equal(404, notfoundResult.StatusCode);    
  }





  [Fact]
  public async Task GetBudget_ShouldReturnOk_WhenUserExistAndIsAuthenticated()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    _dbContext.Budgets.Add(budget);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = GetBudget:
    var result = await _controller.GetBudget(budget.Id);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Budget>>(result);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    Assert.Equal(200, okResult.StatusCode);
    var returnedBudget = Assert.IsType<Budget>(okResult.Value);
    Assert.Equal(returnedBudget.Id, budget.Id);
    Assert.Equal(returnedBudget.Category, budget.Category);
    Assert.Equal(returnedBudget.Limit, budget.Limit);
    Assert.Equal(returnedBudget.UserId, budget.UserId);
  }




  [Fact]
  public async Task GetBudget_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    _dbContext.Budgets.Add(budget);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = GetBudget:
    var result = await _controller.GetBudget(budget.Id);   

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Budget>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode); 

  }





  [Fact]
  public async Task GetBudget_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButIncorrectBudgetId()
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


    var budget = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    _dbContext.Budgets.Add(budget);
    await _dbContext.SaveChangesAsync();

    var incorrect_id = "weofiw";

    // Act: Call the function to test = GetBudget:
    var result = await _controller.GetBudget(incorrect_id);


    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Budget>>(result);
    var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);
    Assert.Equal(404, notFoundResult.StatusCode);    
  }




  [Fact]
  public async Task GetBudgets_ShouldReturnOk_WhenUserExistAndAuthenticated()
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


    var budget1 = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };

    var budget2 = new Budget
    {
      Id = "Budget2",
      Category = "Entertainment",
      Limit = 2000.00M,
      UserId = "12345"
    };


    _dbContext.Budgets.Add(budget1);
    _dbContext.Budgets.Add(budget2);
    await _dbContext.SaveChangesAsync();


    // Act: Call the function to test = GetBudget:
    var result = await _controller.GetBudgets();    

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<IEnumerable<Budget>>>(result);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    Assert.Equal(200, okResult.StatusCode);
    // Extract the list from OkResult:
    var returnedBudgets = Assert.IsType<List<Budget>>(okResult.Value);
    // Look at the count and values:
    Assert.Equal(2, returnedBudgets.Count);
    Assert.Contains(returnedBudgets, b => b.Id == "Budget1" && b.Category == "Entertainment");
    Assert.Contains(returnedBudgets, b => b.Id == "Budget2" && b.Category == "Entertainment");
  }




  [Fact]
  public async Task GetBudgets_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
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


    var budget1 = new Budget
    {
      Id = "Budget1",
      Category = "Entertainment",
      Limit = 32000.00M,
      UserId = "12345"
    };

    var budget2 = new Budget
    {
      Id = "Budget2",
      Category = "Entertainment",
      Limit = 32000.00M,
      UserId = "12345"
    };


    _dbContext.Budgets.Add(budget1);
    _dbContext.Budgets.Add(budget2);
    await _dbContext.SaveChangesAsync();


    // Act: Call the function to test = GetBudget:
    var result = await _controller.GetBudgets();    

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<IEnumerable<Budget>>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);    

  }




}
