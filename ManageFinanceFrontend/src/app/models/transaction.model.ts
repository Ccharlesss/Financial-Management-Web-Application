export interface Transaction {
  id: string;
  description: string;
  date: string;
  amount: number;
  isExpense: boolean;
  financeAccountId: string;
}
