using Microsoft.EntityFrameworkCore;
using ManageFinance.Models;

// Purpose of the Mutation class: Handle the logic for POST, PUT & DELETE:
public class Mutation
{

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








}
