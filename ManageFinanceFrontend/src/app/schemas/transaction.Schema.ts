export interface TransactionSchema {
  Description: string;
  Amount: number;
  Date: string;
  IsExpense: boolean;
  FinanceAccountId: string;
}
