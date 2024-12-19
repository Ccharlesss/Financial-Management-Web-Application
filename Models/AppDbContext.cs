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

    public DbSet<AppUser> AppUsers{get; set;}



    // Purpose: Define the type of relationships btw entities in the DB:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder); // Ensure the base class configuration is applied:

      // Configure relationships:
      modelBuilder.Entity<Transaction>()
        .HasOne(f => f.FinanceAccount)
        .WithMany(g => g.Transactions)
        .HasForeignKey(f => f.FinanceAccountId);

      modelBuilder.Entity<Investment>()
        .HasOne(f => f.User)
        .WithMany(g => g.Investments)
        .HasForeignKey(f => f.UserId);

      modelBuilder.Entity<Goal>()
        .HasOne(f => f.User)
        .WithMany(g => g.Goals)
        .HasForeignKey(f => f.UserId);

      modelBuilder.Entity<Budget>()
        .HasOne(f => f.User)
        .WithMany(g => g.Budgets)
        .HasForeignKey(f => f.UserId)
        .IsRequired(false);

      modelBuilder.Entity<FinanceAccount>()
        .HasOne(f => f.User)
        .WithMany(g => g.Accounts)
        .HasForeignKey(f => f.UserId);

    }

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