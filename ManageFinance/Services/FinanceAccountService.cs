namespace ManageFinance.Services;

public interface IFinanceAccountService
{
  decimal ComputeBalance(FinanceAccount financeAccount);
}

public class FinanceAccountService : IFinanceAccountService
{
  // Implementation of ComputeBalance method of the Interface:
  public decimal ComputeBalance(FinanceAccount financeAccount)
  { // Assess if there is any Transactions in the account:
    if(financeAccount.Transactions == null || !financeAccount.Transactions.Any()) return 0;
    // Case where transactions exist => Sum all transactions (expense:- , income:+):
    return financeAccount.Transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
  }
}