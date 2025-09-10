export interface TransactionSchema {
  Description: string;
  Amount: number;
  Date: string;
  IsExpense: boolean;
  FinanceAccountId: string;
}

export interface UpdateTransactionSchema {
  NewDescription: string;
  NewAmount: number;
  NewDate: string;
  NewStatus: boolean;
}
