using Microsoft.AspNetCore.Identity;

public class AppUser: IdentityUser
{
  public ICollection<FinanceAccount> Accounts { get; set; } = new List<FinanceAccount>();

  public ICollection<Budget> Budgets { get; set; } = new List<Budget>();

  public ICollection<Goal> Goals { get; set; } = new List<Goal>();

  public ICollection<Investment> Investments{ get; set; } = new List<Investment>();
}