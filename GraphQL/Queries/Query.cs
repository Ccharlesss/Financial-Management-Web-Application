using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ManageFinance.Models;
using ManageFinance.Services;
using HotChocolate;
using HotChocolate.Resolvers; 
using HotChocolate.Execution;
public class Query
{
  private readonly UserManager<AppUser> _userManager;

  private readonly RoleManager<IdentityRole> _roleManager;

  private readonly ILogger<Query> _logger;


public Query(
  UserManager<AppUser> userManager,
  RoleManager<IdentityRole> roleManager,
  ILogger<Query> logger)
  
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _logger = logger;
  }


  //=============================================================================================================================
  // Purpose: Handles the logic for listing all users:
  public async Task<List<object>> GetAllUsers()
  { // 1) Fetch all users from the DB:
    var users = await _userManager.Users.ToListAsync();
    // 2) Define a new list that will store users details:
    var userList = new List<object>();
    // 3) Iterate over all users in the users list:
    foreach(var user in users)
    { // 3.1) Fetch the role of the user:
      var roles = await _userManager.GetRolesAsync(user);
      userList.Add(new
      {
        user.Id,
        user.UserName,
        user.Email,
        roles,
      });
    }

    return userList;
  }


  // Purpose: Handles the logic for fetching a user:
  public async Task<object> GetUser(string userId)
  { // 1) Fetch the user from the DB:
    var retrievedUser = await _userManager.FindByIdAsync(userId);
    if(retrievedUser == null)
    {
      _logger.LogWarning($"User with the following userId = {userId} couldnpt be found.");
      return "User not found.";
    }

    // 2) Fetch the role(s) assigned to the user:
    var  retrievedRole = await _userManager.GetRolesAsync(retrievedUser);
    return new
    {
      Id = retrievedUser.Id,
      UserName = retrievedUser.UserName,
      Email = retrievedUser.Email,
      Roles = retrievedRole
    };
  }


  //=============================================================================================================================
  // Purpose: Handles the logic for listing roles:
  public async Task<List<IdentityRole>> GetRoles()
  { // 1) Fetches roles from the AspNetRoles table:
    var roles = await _roleManager.Roles.ToListAsync(); 
    return roles;
  }


  // Purpuse: Handles the logic to fetch a role given its roleId:
  public async Task<IdentityRole> GetRole(string roleId)
  { // 1) Attempt to retrieve the role from the DB:
    var retrievedRole = await _roleManager.FindByIdAsync(roleId);
    if(retrievedRole == null)
    {
      _logger.LogWarning($"The role with the following roleId = {roleId} couldn't be found.");
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage($"Role with the following roleId = {roleId} couldnt be found")
        .SetCode("NOT_FOUND")
        .Build());
    }
    return retrievedRole;
  }

  //=============================================================================================================================








    //=============================================================================================================================
    // Purpose: Handles the logic to fetch all users accounts:
    public async Task<List<FinanceAccount>> GetUserFinanceAccounts(string userId)
    { // 1) Attempt to retrieve the user:
      var retrievedUser = await _userManager.Users
        .Include(u => u.Accounts)
        .FirstOrDefaultAsync(u => u.Id == userId);

      if(retrievedUser == null)
      {
        throw new GraphQLException(ErrorBuilder.New()
          .SetMessage($"User with the userId = {userId} not found.")
          .SetCode("NOT_FOUND")
          .Build());
      }

      // 2) Return the list of finance accounts directly:
      return retrievedUser.Accounts.ToList();
    }

   //=============================================================================================================================
   // Purpose: Handles the logic to fetch user's investments:
   public async Task<List<Investment>> GetUserInvestments(string userId)
   {  // 1) Attempt to fetch the user from the DB:
      var retrievedUser = await _userManager.Users
        .Include(u => u.Investments)
        .FirstOrDefaultAsync(u => u.Id == userId);

      if(retrievedUser == null)
      {
        throw new GraphQLException(ErrorBuilder.New()
          .SetMessage($"User with the following userId = {userId} not found.")
          .SetCode("NOT_FOUND")
          .Build());
      }

      return retrievedUser.Investments.ToList();
   }


   //=============================================================================================================================
   // Purpose: Handles the logic to fetch users Budgets:
  public async Task<List<Budget>> GetUserBudgets(string userId)
  { // 1) Attempt to fetch the user from the DB:
    var retrievedUser = await _userManager.Users
      .Include(u => u.Budgets)
      .FirstOrDefaultAsync(u => u.Id == userId);
    
    if(retrievedUser == null)
    {
      throw new GraphQLException(ErrorBuilder.New()
        .SetMessage($"Users with the following userId = {userId} not found.")
        .SetCode("NOT_FOUND")
        .Build());
    }

    return retrievedUser.Budgets.ToList();
  }


   //=============================================================================================================================
   // Purpose: Handles the logic to fetch users Goals:
   public async Task<List<Goal>> GetUserGoals(string userId)
   {  // 1) Attempt to fetch the user from the DB:
      var retrievedUser = await _userManager.Users
        .Include(u => u.Goals)
        .FirstOrDefaultAsync(u => u.Id == userId);

      if(retrievedUser == null)
      {
        throw new GraphQLException(ErrorBuilder.New()
          .SetMessage($"User with the following userId = {userId} not found.")
          .SetCode("NOT_FOUMD")
          .Build());
      }

      return retrievedUser.Goals.ToList();
   }









  
}