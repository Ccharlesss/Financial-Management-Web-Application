export interface FinanceAccountSchema {
  Id: string;
  AccountName: string;
  AccountType: string;
  UserId: string;
}

export interface CreateAccountSchema {
  UserId: string;
  AccountName: string;
  AccountType: string;
}

export interface UpdateAccountSchema {
  UserId: string;
  AccountName: string;
  AccountType: string;
}
