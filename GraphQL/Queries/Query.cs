using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using ManageFinance.Models;
using ManageFinance.Services;
public class Query
{
  private readonly UserManager<AppUser> _userManager;

  private readonly RoleManager<AppUser> _roleManager;

  private readonly IConfiguration _configuration;

  private readonly ILogger<Query> _logger;


public Query(
  UserManager<AppUser> userManager,
  RoleManager<AppUser> roleManager,
  IConfiguration configuration,
  ILogger<Query> logger)
  
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _configuration = configuration;;
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








  
}