using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ManageFinance.Models;
using ManageFinance.Services;

// Purpose of the Mutation class: Handle the logic for POST, PUT & DELETE:
public class Mutation
{
  private readonly IFinanceAccountService _financeAccountService;
  private readonly UserManager<AppUser> _userManager;
  private readonly SignInManager<AppUser> _signInManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly IConfiguration _configuration;
  private readonly EmailService _emailService;
  private readonly ILogger<Mutation> _logger;

  private readonly IJwtService _jwtService;

  // Dependency injection when the constructor is called:
  public Mutation(
    IFinanceAccountService financeAccountService,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    EmailService emailService,
    ILogger<Mutation> logger,
    IJwtService jwtService)
  {
    _financeAccountService = financeAccountService;
    _userManager = userManager;
    _signInManager = signInManager;
    _roleManager = roleManager;
    _configuration = configuration;
    _emailService = emailService;
    _logger = logger;
    _jwtService = jwtService;
  }


//=============================================================================================================================
  // Purpose: Handle the creation of a Finance Account:
  public async Task<FinanceAccount> CreateFinanceAccount(FinanceAccount input, [Service] ApplicationDbContext context)
  { // 1) Fetch the user:
    var user = await context.Users.FindAsync(input.UserId);
    if (user == null)
    {
      throw new Exception("User not found.");
    }
    // 2) Instantiate a new Finance Account:
    var financeAccount = new FinanceAccount
    {
      AccountName = input.AccountName,
      AccountType = input.AccountType,
      Balance = input.Balance,
      UserId = input.UserId
    };
    // 3) Save the changes to the DB:
    context.Accounts.Add(financeAccount);
    await context.SaveChangesAsync();
    return financeAccount;
  }


  // Purpose: Handles the logic to update a FinanceAccount:
  public async Task<FinanceAccount> UpdateFinanceAccount(string id, FinanceAccount input, [Service] ApplicationDbContext context)
  { // 1) Attempt to retrieve the FinanceAccount to modify:
    var retrievedAccount = await context.Accounts.FindAsync(id);
    if(retrievedAccount == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Financial Account not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Update the states of FinanceAccount properties:
    retrievedAccount.AccountName = input.AccountName;
    retrievedAccount.AccountType = input.AccountType;
    // 3) Change the state of the object to modified to signal EF core to update the entry in the DB:
    context.Entry(retrievedAccount).State = EntityState.Modified;
    // 4) Attempt to save the changes made => where the change happens:
    await context.SaveChangesAsync();
    return retrievedAccount;
  }


  // Purpose: Handles the logic to remove a FinanceAccount:
  public async Task<bool> DeleteFinanceAccount(string id, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the FinanceAccount:
    var retrievedAccount = await context.Accounts.FindAsync(id);
    if(retrievedAccount == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Financial Account not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Change the state of the entry to DELETE to indicated EFcore to update the entry in DB:
    context.Accounts.Remove(retrievedAccount);
    // 3) Attempt to save changes => remove the entry from the DB:
    await context.SaveChangesAsync();
    return true;
  }
//=============================================================================================================================












//=============================================================================================================================
  // Purpose: Handle the creation of an Investment:
  public async Task<Investment> CreateInvestment(Investment input, [Service] ApplicationDbContext context)
  { // 1) Fetch the user: 
    var user = await context.Users.FindAsync(input.UserId);
    if(user ==  null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("User not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Instantiate a new Investment:
    var investment = new Investment
    {
      AssetName = input.AssetName,
      AmountInvested = input.AmountInvested,
      CurrentValue = input.CurrentValue,
      PurchaseDate = input.PurchaseDate,
      UserId = input.UserId,
    };

    // 3) Save the changes to the DB:
    context.Investments.Add(investment);
    await context.SaveChangesAsync();
    return investment;
  }



  // Purpose: Handle the logic to update an Investment:
  public async Task<Investment> UpdateInvestment(Investment input, [Service] ApplicationDbContext context)
  { // 1) Retrieve the Investment to update from DB:
    var retrievedInvestment = await context.Investments.FindAsync(input.Id);
    if(retrievedInvestment == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Investment not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Update the state of the Investment:
    retrievedInvestment.AssetName = input.AssetName;
    retrievedInvestment.AmountInvested = input.AmountInvested;
    retrievedInvestment.CurrentValue = input.CurrentValue;
    retrievedInvestment.PurchaseDate = input.PurchaseDate;
    // 3) Change the state of the entry to modified to indicate EFcore to update the entry in the DB:
    context.Entry(retrievedInvestment).State = EntityState.Modified;
    // 4) Attempt to save the changes made => Where entry is modified:
    await context.SaveChangesAsync();
    return retrievedInvestment;
  }


  // Purpose: Handle the logic to remove an Investment:
  public async Task<bool> DeleteInvestment(string id, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the investment from the DB:
    var retrievedInvestment = await context.Investments.FindAsync(id);
    if(retrievedInvestment == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Investment not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Change the state of the entry to DELETE to indicate to EFcode to delete the entry:
    context.Investments.Remove(retrievedInvestment);
    // 3) Save changes made to the DB => where EFcore removes the entry from DB:
    await context.SaveChangesAsync();
    return true;
  }
//=============================================================================================================================










//=============================================================================================================================
  // Purpose: Handle the logic for the creation of a Goal:
  public async Task<Goal> CreateGoal(Goal input, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the user from the DB:
    var user = await context.Users.FindAsync(input.UserId);
    if(user == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("User not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }
    
    // 2) Instantiate a new Goal:
    var goal = new Goal
    {
      GoalTitle = input.GoalTitle,
      TargetAmount = input.TargetAmount,
      CurrentAmount = input.CurrentAmount,
      TargetDate = input.TargetDate,
      UserId = input.UserId
    };

    // 3) Indicate EFcore to add the new entry to the DB:
    context.Goals.Add(goal);
    // 4) Attempt to commit the changes to the DB => EFcore saves the entry to the DB:
    await context.SaveChangesAsync();
    return goal;
  }


  // Purpose: Handles the logic for updating a Goal:
  public async Task<Goal> UpdateGoal(Goal input, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the Goal to modify from the DB:
    var retrievedGoal = await context.Goals.FindAsync(input.Id);
    if(retrievedGoal == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Goal not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Update the states of the retrieved Goal:
    retrievedGoal.GoalTitle = input.GoalTitle;
    retrievedGoal.TargetAmount = input.TargetAmount;
    retrievedGoal.CurrentAmount = input.CurrentAmount;
    retrievedGoal.TargetDate = input.TargetDate;
    // 3) Change the state of the entry to MODIFIED to indicate EFcore to update the entry to the DB:
    context.Entry(retrievedGoal).State = EntityState.Modified;
    // 4) Attempt to commit the changes made to the DB => EFcore update the entry in the DB:
    await context.SaveChangesAsync();
    return retrievedGoal;
  }



  // Purpose: Handles the logic for removing a Goal:
  public async Task<bool> DeleteGoal(string id, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the goal to remove from the DB:
    var retrievedGoal = await context.Goals.FindAsync(id);
    if(retrievedGoal == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Goal not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Update the state of the entry to DELETE to indicate to EFcore to remove the entry from the DB:
    context.Goals.Remove(retrievedGoal);
    // 3) Attempt to commit the changes made to the DB => EFcore will remove the entry from the DB:
    await context.SaveChangesAsync();
    return true;
  }
//=============================================================================================================================





//=============================================================================================================================
  // Purpose: Handles the logic for the creation of a Budget:
  public async Task<Budget> CreateBudget(Budget input, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the user from the DB:
    var user = await context.Users.FindAsync(input.UserId);
    if(user == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("User not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Instantiate a new Budget:
    var budget = new Budget
    {
      Category = input.Category,
      Limit = input.Limit,
      UserId = input.UserId
    };

    // 3) Update the state of the entry to ADD to indicate EFcore to add the entry to the DB:
    context.Budgets.Add(budget);
    // 4) Attempt to commit the changes made to the DB => EFcore add the Budget to the DB:
    await context.SaveChangesAsync();
    return budget;
  }



  // Purpose: Handles the logic for updating a budget:
  public async Task<Budget> UpdateBudget(Budget input, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the Budget to modify from the DB:
    var retrievedBudget = await context.Budgets.FindAsync(input.Id);
    if(retrievedBudget == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Budget not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Update the fields of the Budget:
    retrievedBudget.Category = input.Category;
    retrievedBudget.Limit = input.Limit;
    // 3) Update the state of the entry to MODIFIED to indicate to EFcore to update the entry in the DB:
    context.Entry(retrievedBudget).State = EntityState.Modified;
    // 4) Attempt to commit the changes made to the DB => EFcore updates the field to the DB:
    await context.SaveChangesAsync();
    return retrievedBudget;
  }



  // Purpose: Handles the logic for removing a Budget:
  public async Task<bool> DeleteBudget(string id, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the Budget to remove from the DB:
    var retrievedBudget = await context.Budgets.FindAsync(id);
    if(retrievedBudget == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Budget not found.")
        .SetCode("NOT_FOUND")
        .Build());
    };

    // 2) Update the state of the entry to DELETE to indicate EFcore to remove the entry from the DB:
    context.Budgets.Remove(retrievedBudget);
    // 3) Attempt to commit changes made to the DB => EFcore removes the entry from the DB:
    await context.SaveChangesAsync();
    return true;
  }
//=============================================================================================================================









//=============================================================================================================================
  // Purpose: Handles the logic for creating a Transaction:
  public async Task<Transaction> CreateTransaction(Transaction input, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the Finance Account from the DB:
    var retrievedFinanceAccount = await context.Accounts
      // FindAsync used to retrieve entity based on PK => not efficient for related entities
      // More efficient other way as it uses a Join thus more efficient
      // one database query is made, fetching both the account and its transactions.
      .FirstOrDefaultAsync(fa => fa.Id == input.FinanceAccountId);
    if(retrievedFinanceAccount == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Finance account not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Instantiate a new Transaction:
    var transaction = new Transaction
    {
      Description = input.Description,
      Amount = input.Amount,
      Date = input.Date,
      IsExpense = input.IsExpense,
      FinanceAccountId = input.FinanceAccountId,
      FinanceAccount = retrievedFinanceAccount
    };

    // 3) Update the state of the entry to ADD to indicate EFcore to add the entry to the DB:
    context.Transactions.Add(transaction);
    // 4) Compute the new balance and update the Financial Account:
    retrievedFinanceAccount.Balance = _financeAccountService.ComputeBalance(retrievedFinanceAccount);
    await context.SaveChangesAsync();
    return transaction;
  }


  // Purpose: Handles the logic for updating a Transaction:
  public async Task<Transaction> UpdateTransaction(Transaction input, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the transaction to update from the DB:
    var retrievedTransaction = await context.Transactions
      .Include(t => t.FinanceAccount)
      .FirstOrDefaultAsync(t => t.Id == input.Id);
    if(retrievedTransaction == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Transaction not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Update the fields of the retrieved transaction:
    retrievedTransaction.Description = input.Description;
    retrievedTransaction.Amount = input.Amount;
    retrievedTransaction.Date = input.Date;
    retrievedTransaction.IsExpense = input.IsExpense;
    // 3) Update the state of the entry to MODIFIED to indicate EFcore to update the entry in the DB:
    context.Entry(retrievedTransaction).State = EntityState.Modified;

    // 4) Update the balance of the finance account:
    retrievedTransaction.FinanceAccount.Balance = _financeAccountService.ComputeBalance(retrievedTransaction.FinanceAccount);
    // 5) Attempt to commit changes made to the DB:
    await context.SaveChangesAsync();
    return retrievedTransaction;
  }


  // Purpose: Handles the logic for deleting a Transaction:
  public async Task<bool> DeleteTransaction(string id, [Service] ApplicationDbContext context)
  { // 1) Attempt to fetch the Transaction to remove from the DB:
    var retrievedTransaction = await context.Transactions
      .Include(t => t.FinanceAccount)
      .FirstOrDefaultAsync(t => t.Id == id);
    if(retrievedTransaction == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage("Transaction not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    // 2) Update the state of the entry to DELETE to indicate EFcore to remove the entry from the DB:
    context.Transactions.Remove(retrievedTransaction);

    // 3) Update the balance of the retrieved Finance account:
    retrievedTransaction.FinanceAccount.Balance = _financeAccountService.ComputeBalance(retrievedTransaction.FinanceAccount);
    // 4) Attempt to commit changes made to the DB:
    await context.SaveChangesAsync();
    return true;
  }





//=============================================================================================================================
  // Purpose: Handles the logic for Account creation:
  public async Task<string> RegisterUser(AuthSchema input)
  { // 1) Create an instance of AppUser:
    var user = new AppUser {UserName = input.Email, Email = input.Email};
    // 2) Attempt to create a new AppUser in the system using 'UserManager<AppUser>' service:
    var result = await _userManager.CreateAsync(user, input.Password);

    if(result.Succeeded)
    { 
      _logger.LogInformation("User successfully created: {UserId} {Email}", user.Id, input.Email);
      // 3) Ensure the role exist in the AspNetRole table:
      var roleExist = await _roleManager.RoleExistsAsync("User");

      if(!roleExist)
      { 
        _logger.LogWarning("Role 'User' does not exist. Creating it now.");
        // 3.1) Create the role in the AspNetRole table:
        var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));

        if(!roleResult.Succeeded)
        {
          _logger.LogError("Failed to create role 'User'. Errors: {Errors}", 
            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
          return "Failed to create role 'User'.";
        }
      }

      // 3.1) Assign the role = user to the newly created user:
      var roleAssigned = await _userManager.AddToRoleAsync(user, "User");
      if(!roleAssigned.Succeeded)
      {
        _logger.LogError("Failed to assign role 'User' to user {UserId} ({Email}). Errors: {Errors}", 
            user.Id, input.Email, string.Join(", ", roleAssigned.Errors.Select(e => e.Description)));
        return "Failed to assign role 'User' to the newly created user.";
      }
      _logger.LogInformation("Role 'User' successfully assigned to user {UserId} ({Email}).", user.Id, input.Email);

      // 4) Generate an Email verification token to the newly created user:
      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      // 5) URL-encode the token to make it safe to use in a URL query string:
      var encodedToken = System.Net.WebUtility.UrlEncode(token);
      // 6) Create the verification link with the encoded token:
      string baseUrl = "https://yourfrontend.com";
      string verificationLink = $"{baseUrl}/verify-email?userId={user.Id}&token={encodedToken}";
      // 7) Send the verification link to the email of the user:
      _emailService.SendEmail(user.Email, "Email Verification", $"Verify your email: {verificationLink}");
      _logger.LogInformation("User registration process completed successfully for {Email}.", user.Email);
      return "User registered successfully. Email verification link sent.";
    }
    return string.Join(", ", result.Errors.Select(e => e.Description));
  }


  // Purpose: Handles the logic for signing new users in:
  public async Task<string> login(AuthSchema input)
  { // 1) Attempt to sign in with the user's credentials:
    var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, isPersistent:false, lockoutOnFailure:false);

    if(!result.Succeeded)
    { _logger.LogInformation("invalig login attempt. Either the username or password is incorrect.");
      return "Invalid login attempt.";
    }

    // 2) Retrieve the user's data:
    var user = await _userManager.FindByEmailAsync(input.Email);
    if(user == null)
    {
      return "User not found.";
    }
    
    // 3) Get the role:
    var roles=  await _userManager.GetRolesAsync(user);
    // 4) Generate a JWT token for the user:
    var token = _jwtService.GenerateJwtToken(user, roles);
    return token;
  }


  // Purpose: Handles the logic for loging out users:
  public async Task<string> Logout()
  {
    // Sign out the user from the cookie-based authentication system:
    await _signInManager.SignOutAsync();
    return "Logged out successfully.";
  }


  // Purpose: Handles the logic for updating user's password:
  public async Task<string> updatePassword(UpdatePasswordSchema input)
  { // 1) Attempt to retrieve user from the DB:
    var user = await _userManager.FindByEmailAsync(input.Email);
    if(user == null)
    {
      return "User not found.";
    }

    // 2) Attempt to reset user's password:
    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var result = await _userManager.ResetPasswordAsync(user, token, input.NewPassword);
    if(result.Succeeded)
    {
      return "Password changed successfully";
    }
    return string.Join(", ", result.Errors.Select(e => e.Description));
  }



  // Purpose: Handles the logic for verifying user's email when he signs up:
  public async Task<string> VerifyEmail(string userId, string token)
  { // 1) Decode the token received:
    var decodedToken = System.Net.WebUtility.UrlDecode(token);
    // 2) Retrieve the user corresponding to the userID:
    var user = await _userManager.FindByIdAsync(userId);
    if(user == null)
    {
      return "User not found.";
    }

    // 3) Attempt to verify that the email confirmation token matches the user:
    var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
    if(result.Succeeded)
    {
      return "Email verification successful.";
    }

    return string.Join(", ", result.Errors.Select(e => e.Description));
  }
//=============================================================================================================================












//=============================================================================================================================
  // Purpose: Handles the logic for creating a role:
  public async Task<string> CreateRole(CreateRoleSchema input)
  { // 1) Assess if a RoleName was provided:
    if(string.IsNullOrEmpty(input.RoleName))
    {
      return "RoleName field is required.";
    }

    // 2) Instantiate a new IdentityRole:
    var role = new IdentityRole(input.RoleName);
    // 3) Attempt to create the new role:
    var result = await _roleManager.CreateAsync(role);
    if(result.Succeeded)
    {
      return "Role successfully created.";
    }

    return string.Join(", " ,result.Errors.Select(e => e.Description));
  }



  // Purpose: Handles the logic for updating user's role:
  public async Task<string> UpdateRole(UpdateRoleSchema input)
  { // 1) Attempt to retrieve the role to update from the DB:
    var retrievedRole = await _roleManager.FindByIdAsync(input.RoleId);
    if(retrievedRole == null)
    {
      return "Role not found.";
    }

    // 2) Update the role:
    retrievedRole.Name = input.NewRoleName;
    // 3) Attempt to commit the changes made to the role name:
    var result = await _roleManager.UpdateAsync(retrievedRole);
    if(result.Succeeded)
    {
      return "Role successfully updated.";
    }

    return string.Join(", ", result.Errors.Select(e => e.Description));


  }
















}
