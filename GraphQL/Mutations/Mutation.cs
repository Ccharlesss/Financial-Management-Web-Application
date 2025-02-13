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








}
