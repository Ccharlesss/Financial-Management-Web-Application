using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ManageFinance.Models
{
  public class ApplicationDbContext: IdentityDbContext<IdentityUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Transaction> Transactions {get; set;}

  }
  
}