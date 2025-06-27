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

public class GoalControllerTests
{
  private readonly GoalsController _controller;

  private readonly ApplicationDbContext _dbContext;


  public GoalControllerTests()
  {
    // Configure an in-memory DB:
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase( Guid.NewGuid().ToString())
    .Options;

    // Create a DB context using the in-memory database to store & retrieve data during testing:
    _dbContext = new ApplicationDbContext(options);

    // Inject the in memory DB into the GoalController for testing:
    _controller = new GoalsController(_dbContext);

  }



  [Fact]
  public async Task PostGoal_ShouldReturnCreatedResult_WhenUserExistAndLoggedIn()
  { // Arrange: Set up the test environment:
    // A) Create a fake user in the DB:
    var user = new AppUser {Id = "12345", UserName = "testUser@example.com", Email = "testUser@example.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // B) Set up the controller's context to sumulate an authenticated user:
    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Name, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

    var identity = new ClaimsIdentity(claims, "test");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = claimsPrincipal
      }
    };

    // C) Create a goal obj with the mock userId:
    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a car",
      TargetAmount = 10000,
      CurrentAmount = 2400,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = user.Id
    };

    // Act: Call the PostGoal method:
    var result = await _controller.PostGoal(goal);


    // Assert: Verify result type and status code:
    // 1) Assess the returned wrapper is of type: ActionResult<Goal>:
    var actionResult = Assert.IsType<ActionResult<Goal>>(result);
    // 2) Assess the Response in the ActionResult wrapper:
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
    Assert.Equal(201, createdAtActionResult.StatusCode);
    // Verify the goal was added to the database
    var savedGoal = await _dbContext.Goals.FindAsync(goal.Id);
    Assert.NotNull(savedGoal);
    Assert.Equal(goal.TargetAmount, savedGoal.TargetAmount);
    Assert.Equal(goal.CurrentAmount, savedGoal.CurrentAmount);
    Assert.Equal(goal.GoalTitle, savedGoal.GoalTitle);
    Assert.Equal(goal.UserId, savedGoal.UserId);
  }


  [Fact]
  public async Task PostGoal_ShouldReturnUnAuthorized_WhenUserExistAndIsNotLoggedIn()
  { // Arrange: Set up the test environment:
    var user = new AppUser {Id = "12345", UserName = "example@test.com", Email = "example@test.com"};
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "save for a plane",
      TargetAmount = 34134,
      CurrentAmount = 1343,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = user.Id
    };

    _controller.ControllerContext = new ControllerContext
    {
      HttpContext = new DefaultHttpContext
      {
        User = null
      }
    };

    // Act: Call the function to test = PostGoal()
    var result = await _controller.PostGoal(goal);

    // Assert: Verify the result type and status code:
    var actionResult = Assert.IsType<ActionResult<Goal>>(result);
    Assert.IsType<UnauthorizedResult>(actionResult.Result);
  }



  [Fact]
  public async Task PostGoal_ShouldReturnNotFound_WhenInvalidUserCredentials()
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

    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a jet",
      TargetAmount = 100000,
      CurrentAmount = 2400,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12342r"
    };

    // Act: Call the method to test = PostGoal:
    var result = await _controller.PostGoal(goal);

    // Assert Verify the result & status code:
    var actionResult = Assert.IsType<ActionResult<Goal>>(result);
    Assert.IsType<NotFoundObjectResult>(actionResult.Result);
  }




  [Fact]
  public async Task DeleteGoal_ShouldReturnNoContent_WhenUserExistAndIsAuthenticated()
  { // Act: Set up the test environment:
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

    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a car",
      TargetAmount = 1000000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = DeleteGoal:
    var result = await _controller.DeleteGoal(goal.Id);

    // Assert: Verify the result and the status code:
    var NoContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, NoContentResult.StatusCode);
  }



  [Fact]
  public async Task DeleteGoal_ShouldReturnNotAuthorized_WhenUserExistAndIsNotAuthenticated()
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

    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for Plane",
      TargetAmount = 1000000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = DeleteGoal:
    var result = await _controller.DeleteGoal(goal.Id);

    // Assert: Assess the response and the status code:
    var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, UnauthorizedResult.StatusCode);
  }





  [Fact]
  public async Task DeleteGoal_ShouldReturnNotFound_WhenUserIsLoggedInButGoalIdIncorrect()
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

    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for plane",
      TargetAmount = 1000000,
      CurrentAmount = 1000,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal);
    await _dbContext.SaveChangesAsync();

    var incorrectGoalId = "Goal2";

    // Act: Call the function to test = DeleteGoal:
    var result = await _controller.DeleteGoal(incorrectGoalId);

    // Assert: Assess the response and status code:
    var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal(404, NotFoundResult.StatusCode);
  }



  [Fact]
  public async Task PutGoal_ShouldReturnNoContent_WhenUserExistAndAuthenticatedAndGoalDataCorrect()
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

    var originalGoal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a plane",
      TargetAmount = 1000000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(originalGoal);
    await _dbContext.SaveChangesAsync();

    var updatedGoal = new Goal
    {
      GoalTitle = "Save for vacation",
      TargetAmount = 2000000,
      CurrentAmount = 200,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    // Act: Call the function to test:
    var result = await _controller.PutGoal(originalGoal.Id, updatedGoal);

    // Assert: Assess the response and status code:
    var NoContentResult = Assert.IsType<NoContentResult>(result);
    Assert.Equal(204, NoContentResult.StatusCode);
  }


  [Fact]
  public async Task PutGoal_ShouldReturnUnauthorized_WhenUserExistAndNotAuthenticated()
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

    var originalGoal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a car",
      TargetAmount = 1000000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(originalGoal);
    await _dbContext.SaveChangesAsync();

    var updatedGoal = new Goal
    {
      GoalTitle = "Save for vacation",
      TargetAmount = 2000000,
      CurrentAmount = 200,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    // Act: Call the function to test:
    var result = await _controller.PutGoal(originalGoal.Id, updatedGoal);

    // Assert: Assess the response and status code:
    var UnauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
    Assert.Equal(401, UnauthorizedResult.StatusCode);
  }



  [Fact]
  public async Task PutGoal_ShouldReturnNotFound_WhenUSerExistAndIsAuthenticatedButGoalIdIncorrect()
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


    var originalGoal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a car",
      TargetAmount = 1000000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(originalGoal);
    await _dbContext.SaveChangesAsync();

    var updatedGoal = new Goal
    {
      GoalTitle = "Save for vacation",
      TargetAmount = 2000000,
      CurrentAmount = 200,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    var incorrectGoalId = "2eweir";

    // Act: Call the function to test = PutGoal:
    var result = await _controller.PutGoal(incorrectGoalId, updatedGoal);

    // Assert: Assess the response and status code:
    var NotFoundResult = Assert.IsType<NotFoundObjectResult>(result);
    Assert.Equal(404, NotFoundResult.StatusCode);
  }


  [Fact]
  public async Task GetGoal_ShouldReturnGoal_WhenUserExistAndIsAuthenticatedAndCorrectGoalId()
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

    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a trip to Tokyo",
      TargetAmount = 5000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = GetGoal:
    var result = await _controller.GetGoal(goal.Id);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Goal>>(result);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    var returnedGoal = Assert.IsType<Goal>(okResult.Value);
    Assert.Equal(goal.Id, returnedGoal.Id);
    Assert.Equal(goal.GoalTitle, returnedGoal.GoalTitle);
    Assert.Equal(goal.TargetAmount, returnedGoal.TargetAmount);
    Assert.Equal(goal.CurrentAmount, returnedGoal.CurrentAmount);
    Assert.Equal(goal.TargetDate, returnedGoal.TargetDate);
    Assert.Equal(goal.UserId, returnedGoal.UserId);
  }




  [Fact]
  public async Task GetGoal_ShouldReturnUnauthorized_WhenUserExistAndNotAuthenticated()
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


    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a trip to Tokyo",
      TargetAmount = 5000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = GetGoal:
    var result = await _controller.GetGoal(goal.Id);

    // Assert: Assess the response:
    var actionResult = Assert.IsType<ActionResult<Goal>>(result);
    var UnautorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
  }



  [Fact]
  public async Task GetGoal_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButGoalIdIncorrect()
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

    var goal = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a trip to Tokyo",
      TargetAmount = 5000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal);
    await _dbContext.SaveChangesAsync();

    var incorrectId = "wrwjbwei2";

    // Act: Call the function to test = GetGoal:
    var result = await _controller.GetGoal(incorrectId);

    // Assert: Assess the response:
    var actionResult = Assert.IsType<ActionResult<Goal>>(result);
    var NotFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
  }



  [Fact]
  public async Task GetGoals_ShouldReturnListOfGoals_WhenUserExistAndIsAuthenticated()
  { // Act: Set up the test environment:
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

    var goal1 = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a trip to Tokyo",
      TargetAmount = 5000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };


    var goal2 = new Goal
    {
      Id = "Goal2",
      GoalTitle = "Save for a trip to LosAngeles",
      TargetAmount = 51000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal1);
    _dbContext.Goals.Add(goal2);
    await _dbContext.SaveChangesAsync();

    // Act: Call the function to test = GetGoals:
    var result = await _controller.GetGoals();

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<IEnumerable<Goal>>>(result);
    var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
    // Extract the list from okResult:
    var returnedGoals = Assert.IsType<List<Goal>>(okResult.Value);
    // Look at the count and values:
    Assert.Equal(2, returnedGoals.Count);
    Assert.Contains(returnedGoals, g => g.Id == "Goal1" && g.GoalTitle == "Save for a trip to Tokyo");
    Assert.Contains(returnedGoals, g => g.Id == "Goal2" && g.GoalTitle == "Save for a trip to LosAngeles");
  }



  [Fact]
  public async Task GetGoals_ShouldReturnUnauthorized_WhenUserExistAndIsNotAuthenticated()
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

    var goal1 = new Goal
    {
      Id = "Goal1",
      GoalTitle = "Save for a trip to Tokyo",
      TargetAmount = 5000,
      CurrentAmount = 100,
      TargetDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
      UserId = "12345"
    };

    _dbContext.Goals.Add(goal1);
    await _dbContext.SaveChangesAsync();


    // Act: Call the function to test:
    var result = await _controller.GetGoals();

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<IEnumerable<Goal>>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
  }













}