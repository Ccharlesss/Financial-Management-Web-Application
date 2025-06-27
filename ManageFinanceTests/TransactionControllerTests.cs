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



public class TransactionControllerTests
{
  private readonly TransactionsController _controller;

  private readonly Mock<IFinanceAccountService> _mockfinanceAccountService;

  private readonly ApplicationDbContext _dbContext;

  public TransactionControllerTests()
  { // Configure the options for an in-memory DB:
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;

    // Create the mock in-memory DB:
    _dbContext = new ApplicationDbContext(options);

    // Mock the IFinanceAccountService:
    _mockfinanceAccountService = new Mock<IFinanceAccountService>();

    // Inject the dependencies into the controller:
    _controller = new TransactionsController(_dbContext, _mockfinanceAccountService.Object);
  }


  [Fact]
  public async Task PostTransaction_ShouldReturnCreatedAtAction_WhenUserExistAndAuthenticatedAndFinanceAccountExist()
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

    var transaction = new Transaction
    {
        Id = "tx1",
        FinanceAccountId = "FinanceAccount1",
        Description = "Capital gain from crypto",
        Amount = 200.00M,
        Date = DateOnly.FromDateTime(DateTime.Now),
        IsExpense = false
    };


    _mockfinanceAccountService
      .Setup(s => s.ComputeBalance(It.IsAny<FinanceAccount>()))
      .Returns(financeAccount.Balance + transaction.Amount);

    // Act: Call the function to test = PostTransaction:
    var result = await _controller.PostTransaction(transaction);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Transaction>>(result);
    var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
    Assert.Equal(201, createdAtActionResult.StatusCode);

    var retrievedAccount = await _dbContext.Accounts.FindAsync(financeAccount.Id);
    Assert.Equal(retrievedAccount.Balance, transaction.Amount);

    var retrievedTransaction = await _dbContext.Transactions.FindAsync(transaction.Id);
    Assert.Equal(retrievedTransaction.Id, transaction.Id);
    Assert.Equal(retrievedTransaction.FinanceAccountId, transaction.FinanceAccountId);
    Assert.Equal(retrievedTransaction.Description, transaction.Description);
    Assert.Equal(retrievedTransaction.Amount, transaction.Amount);
    Assert.Equal(retrievedTransaction.Date, transaction.Date);
    Assert.Equal(retrievedTransaction.IsExpense, transaction.IsExpense);

  }




  [Fact]
  public async Task PostTransaction_ShouldReturnUnauthorized_WhenUserExistButNotAuthenticated()
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

    var transaction = new Transaction
    {
        Id = "tx1",
        FinanceAccountId = "FinanceAccount1",
        Description = "Capital gain from crypto",
        Amount = 200.00M,
        Date = DateOnly.FromDateTime(DateTime.Now),
        IsExpense = false
    };


    _mockfinanceAccountService
      .Setup(s => s.ComputeBalance(It.IsAny<FinanceAccount>()))
      .Returns(financeAccount.Balance + transaction.Amount);

    // Act: Call the function to test = PostTransaction:
    var result = await _controller.PostTransaction(transaction);

    // Assert:
    var actionResult = Assert.IsType<ActionResult<Transaction>>(result);
    var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
    Assert.Equal(401, unauthorizedResult.StatusCode);
  }




  [Fact]
  public async Task PostTransaction_ShouldReturnNotFound_WhenUserExistAndAuthenticatedButAccountDontExist()
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

    var transaction = new Transaction
    {
        Id = "tx1",
        FinanceAccountId = "InvalidAccountId",
        Description = "Capital gain from crypto",
        Amount = 200.00M,
        Date = DateOnly.FromDateTime(DateTime.Now),
        IsExpense = false
    };


    _mockfinanceAccountService
      .Setup(s => s.ComputeBalance(It.IsAny<FinanceAccount>()))
      .Returns(financeAccount.Balance + transaction.Amount);

    // Act: Call the function to test = PostTransaction:
    var result = await _controller.PostTransaction(transaction);

    // Assert: Assess the response and status code:
    var actionResult = Assert.IsType<ActionResult<Transaction>>(result);
    var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
    Assert.Equal(404, notFoundResult.StatusCode);
 

  }






}