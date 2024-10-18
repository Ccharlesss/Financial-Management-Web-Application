using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ManageFinance.Models
{
  public class ApplicationDbContext: IdentityDbContext<AppUser>
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    // DbSet<T> : Collection of entities of a specific type (T) that can be queried from the database and persisted to it
    // Each DbSet represents a table in the database. && provides methods for Create, Read, Update, and Delete (CRUD) operations.
    public DbSet<Transaction> Transactions {get; set;}

    public DbSet<Investment> Investments {get; set;}

    public DbSet<Goal> Goals {get; set;}

    public DbSet<FinanceAccount> Accounts {get; set;}

    public DbSet<Budget> Budgets {get; set;}

  }
  
}






// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.AspNetCore.Identity;

// namespace ManageFinance.Models
// {
//   public class ApplicationDbContext: DbContext
//   {
//     public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
//     {

//     }

//     // DbSet<T> : Collection of entities of a specific type (T) that can be queried from the database and persisted to it
//     // Each DbSet represents a table in the database. && provides methods for Create, Read, Update, and Delete (CRUD) operations.
//     public DbSet<Transaction> Transactions {get; set;}

//     public DbSet<Investment> Investments {get; set;}

//     public DbSet<Goal> Goals {get; set;}

//     public DbSet<FinanceAccount> Accounts {get; set;}

//     public DbSet<Budget> Budgets {get; set;}

//   }
  
// }